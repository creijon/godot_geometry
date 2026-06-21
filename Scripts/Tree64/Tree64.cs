using System;
using System.Collections.Generic;
using Godot;

// Depth 0: Blocks
// Depth 1: Single layer of payload references (blocks * 4)
// Depth 2: 1 layer of internal tree and then payload (blocks * 16)
// Depth 3: 2 layers of internal tree and then payload (blocks * 64)

public class Tree64
{
    // Size and subdivisions
    Geo3D.AABB _bounds;
    Int3 _divisions;
    int _depth;
    Vector3 _blockExtents;
    Vector3 _blockInvSize;

    struct BlockEntry
    {
        public uint tree;
        public uint payload;
    }

    // CPU copy of the GPU buffers.
    BlockEntry[] _blockTable;
    uint[] _blockTrees;
    uint[] _blockPayloads;
    int _totalTreeSize;
    int _totalPayloadSize;

    const uint EMPTY_CELL = 0xFFFFFFFF;

    Int3 FindBlockCell(Vector3 pos)
    {
        var relPos = pos - _bounds.Min;

        // Find the child which the pos is inside.
        var i = Mathf.Clamp(Mathf.FloorToInt(relPos.X * _blockInvSize.X), 0, _divisions.X - 1);
        var j = Mathf.Clamp(Mathf.FloorToInt(relPos.Y * _blockInvSize.Y), 0, _divisions.Y - 1);
        var k = Mathf.Clamp(Mathf.FloorToInt(relPos.Z * _blockInvSize.Z), 0, _divisions.Z - 1);

        return new Int3(i, j, k);
    }

    Int3 FindChildCell(Vector3 pos, Geo3D.AABB bounds)
    {
        var relPos = pos - bounds.Min;

        // Find the child cell that contains pos.
        var i = Mathf.Clamp(Mathf.FloorToInt(relPos.X * 2.0f / bounds.extents.X), 0, 3);
        var j = Mathf.Clamp(Mathf.FloorToInt(relPos.Y * 2.0f / bounds.extents.Y), 0, 3);
        var k = Mathf.Clamp(Mathf.FloorToInt(relPos.Z * 2.0f / bounds.extents.Z), 0, 3);

        return new Int3(i, j, k);
    }

    Int3 GetBlockCell(int cellID)
    {
        int z = cellID / (_divisions.Y * _divisions.X);
        int remainderZ = cellID - (z * _divisions.Y * _divisions.X);
        int y = remainderZ / _divisions.X;
        int x = remainderZ - (y * _divisions.X);

        return new Int3(x, y, z);
    }

    int GetBlockID(Int3 blockCell)
    {
        return blockCell.X + (blockCell.Y + blockCell.Z * _divisions.Y) * _divisions.X;
    }

    Vector3 GetBlockCentre(Int3 blockCell)
    {
        return (Vector3)blockCell * (_blockExtents * 2.0f) + _blockExtents + _bounds.Min;
    }

    bool TestChildBit(uint nodeIndex, int childID)
    {
        uint lword = (childID < 32) ? (uint)(_blockTrees[nodeIndex] & (1 << childID)) : 0;
        uint hword = (childID < 32) ? 0 : (uint)(_blockTrees[nodeIndex + 1] & (1 << (childID - 32)));

        return (lword | hword) != 0;
    }

    uint GetChildSlot(uint nodeIndex, int childID)
    {
#if true
        // Mask out the bits above the childID for low and high words.
        uint maskL = (childID < 32) ? (0x7FFFFFFFu >> (31 - childID)) : 0xFFFFFFFFu;
        uint maskH = (childID < 32) ? 0 : (0x7FFFFFFFu >> (63 - childID));

        // Add the popcounts of the masked words.
        return BitOps.PopCount(_blockTrees[(int)nodeIndex] & maskL)
             + BitOps.PopCount(_blockTrees[(int)nodeIndex + 1] & maskH);
#else
        // With 64-bit int support
        ulong x = _blockTrees[(int)nodeIndex] | ((ulong)_blockTrees[(int)nodeIndex + 1] << 32);

        x &= ((1UL << childID) - 1);

        x -= ((x >> 1) & 0x5555555555555555ul);
        x = (x & 0x3333333333333333ul) + (x >> 2 & 0x3333333333333333ul);
        x = (x + (x >> 4)) & 0xf0f0f0f0f0f0f0ful;
        x = (x * 0x0101010101010101ul) >> 56;

        return (uint)x;
#endif
    }

    uint GetChildIndex(uint nodeIndex, uint childSlot)
    {
        return nodeIndex + _blockTrees[(int)(nodeIndex + 2 + childSlot)];
    }

    public static int GetChildID(Int3 childCell)
    {
        return childCell.X + (childCell.Y << 2) + (childCell.Z << 4);
    }

    public static Int3 GetChildCell(int childID)
    {
        return new Int3((childID >> 0) & 3,
                        (childID >> 2) & 3,
                        (childID >> 4) & 3);
    }

    public static Vector3 GetChildMin(Geo3D.AABB parentAABB, Int3 childCell)
    {
        var childSize = parentAABB.extents * 0.5f;
        return parentAABB.Min + (Vector3)childCell * childSize;
    }

    public static Geo3D.AABB GetChildBounds(Geo3D.AABB parentAABB, Int3 childCell)
    {
        var childSize = parentAABB.extents * 0.5f;
        var childMin = parentAABB.Min + (Vector3)childCell * childSize;
        var childMax = childMin + childSize;

        return new Geo3D.AABB(childMin, childMax, true);
    }

    public Geo3D.AABB Bounds
    {
        get { return _bounds; }
    }

    public void Initialise(Geo3D.AABB bounds, Int3 divisions, int depth, List<uint>[] blockTrees, List<uint>[] blockPayloads)
    {
        // Copy the block trees over and repack.
        _bounds = bounds;
        _divisions = divisions;
        _depth = depth;
        _blockExtents = new Vector3(_bounds.extents.X / _divisions.X,
                                    _bounds.extents.Y / _divisions.Y,
                                    _bounds.extents.Z / _divisions.Z);
        _blockInvSize = new Vector3(_divisions.X / _bounds.Size.X,
                                    _divisions.Y / _bounds.Size.Y,
                                    _divisions.Z / _bounds.Size.Z);
        _blockTable = new BlockEntry[divisions.Total];    // Pairs of offset to tree and payloads

        _totalTreeSize = 0;
        _totalPayloadSize = 0;

        for (int i = 0; i < divisions.Total; ++i)
        {
            var block = blockTrees[i];

            if (block != null)
            {
                _blockTable[i].tree = (uint)_totalTreeSize;
                _blockTable[i].payload = (uint)_totalPayloadSize;
                _totalTreeSize += block.Count;
                _totalPayloadSize += blockPayloads[i].Count;
            }
            else
            {
                _blockTable[i].tree = EMPTY_CELL;
                _blockTable[i].payload = EMPTY_CELL;
            }
        }

        _blockTrees = new uint[_totalTreeSize];
        _blockPayloads = new uint[_totalPayloadSize];

        for (int i = 0; i < divisions.Total; ++i)
        {
            var tree = blockTrees[i];
            if (tree != null)
            {
                Array.Copy(tree.ToArray(), 0, _blockTrees, _blockTable[i].tree, tree.Count);
            }
            var payload = blockPayloads[i];
            if (payload != null)
            {
                Array.Copy(payload.ToArray(), 0, _blockPayloads, _blockTable[i].payload, payload.Count);
            }
        }
    }

    uint FindBlockPayload(Vector3 localPos, uint blockIndex, Vector3 blockCentre, ref Int3 hitCell)
    {
        uint nodeIndex = blockIndex;
        var nodeMin = blockCentre - _blockExtents;
        var childSize = _blockExtents * 0.5f;;
        var childInvSize = _blockInvSize * 4.0f;

        for (int depth = 0; depth < _depth; ++depth)
        {
            var cellPos = (localPos - nodeMin) * childInvSize;
            var childCell = new Int3(Mathf.FloorToInt(cellPos.X),
                                     Mathf.FloorToInt(cellPos.Y),
                                     Mathf.FloorToInt(cellPos.Z));
            int childID = GetChildID(childCell);

            hitCell = (hitCell * 4) + childCell;

            if (!TestChildBit(nodeIndex, childID)) return EMPTY_CELL;

            uint childSlot = GetChildSlot(nodeIndex, childID);
            nodeIndex = GetChildIndex(nodeIndex, childSlot);
            nodeMin = nodeMin + (Vector3)childCell * childSize;
            childSize *= 0.25f;
            childInvSize *= 4.0f;
        }

        return nodeIndex;
    }

    public bool Intersect(Geo3D.Ray ray, ref Int3 hitCell, Transform3D transform)
    {
        bool hit = false;
        Int3 blockCell = Int3.Zero;
        var invTransform = transform.Inverse();
        var localRay = new Geo3D.Ray(invTransform * ray.origin, invTransform * ray.dir);

        if (Geo3D.Intersect.Test(localRay.origin, Bounds))
        {
            // Ray starts inside the bounds.
            blockCell = FindBlockCell(localRay.origin);
            hitCell = blockCell;

            uint blockRoot = _blockTable[GetBlockID(blockCell)].tree;

            if (blockRoot != EMPTY_CELL)
            {
                var blockMin = GetBlockCentre(blockCell) - _blockExtents;

                // Find whether the origin is in an occupied leaf payload.
                uint payloadOffset = FindBlockPayload(localRay.origin, blockRoot, blockMin, ref hitCell);

                if (payloadOffset != EMPTY_CELL)
                {
                    hit = true;
                }
            }
        }
        else
        {
            float t;
            hit = Geo3D.Intersect.Test(localRay, Bounds, out t);

            if (hit)
            {
                var dir = localRay.dir;
                // Handle rays along the cardinal axes, where 1/dir will be NaN for 0 components.
                var invDir = new Vector3((dir.X != 0.0f) ? (1.0f / dir.X) : Mathf.Inf,
                                         (dir.Y != 0.0f) ? (1.0f / dir.Y) : Mathf.Inf,
                                         (dir.Z != 0.0f) ? (1.0f / dir.Z) : Mathf.Inf);
                Int3 step = new Int3((dir.X > 0.0f) ? 1 : -1,
                                     (dir.Y > 0.0f) ? 1 : -1,
                                     (dir.Z > 0.0f) ? 1 : -1);

                // Check for intersection with blocks.
                var hitPos = localRay.Position(t);
                blockCell = FindBlockCell(hitPos);

                hit = false;

                while (true)
                {
                    int blockID = GetBlockID(blockCell);
                    var blockCentre = GetBlockCentre(blockCell);
                    uint blockRoot = _blockTable[blockID].tree;

                    // Recurse into block if it has a tree.
                    if (blockRoot != EMPTY_CELL)
                    {
                        var blockBounds = new Geo3D.AABB(blockCentre, _blockExtents);
                        hitCell = blockCell;

                        if (IntersectTree(hitPos, localRay.dir, invDir, step, blockRoot, blockBounds, 1, ref hitCell))
                        {
                            hit = true;
                            break;
                        }
                    }

                    // If the tree wasn't intersected then walk to next block.
                    var border = blockCentre + Vector3.Scale((Vector3)step, _blockExtents);
                    var s = Vector3.Scale(border - hitPos, invDir);

                    if (s.X < s.Y && s.X < s.Z)
                    {
                        blockCell.X += step.X;
                        hitPos += dir * s.X;
                        hitPos.X = border.X;    // Snap to X border for accuracy.
                        if (blockCell.X < 0 || blockCell.X >= _divisions.X) break;
                    }
                    else if (s.Y < s.Z)
                    {
                        blockCell.Y += step.Y;
                        hitPos += dir * s.Y;
                        hitPos.Y = border.Y;    // Snap to Y border for accuracy.
                        if (blockCell.Y < 0 || blockCell.Y >= _divisions.Y) break;
                    }
                    else
                    {
                        blockCell.Z += step.Z;
                        hitPos += dir * s.Z;
                        hitPos.Z = border.Z;    // Snap to Z border for accuracy.
                        if (blockCell.Z < 0 || blockCell.Z >= _divisions.Z) break;
                    }
                }
            }
        }

        return hit;
    }

    bool IntersectTree(Vector3 pos, Vector3 dir, Vector3 invDir, Int3 step,
                       uint nodeIndex, Geo3D.AABB nodeBounds, int depth, ref Int3 hitCell)
    {
        var childCell = FindChildCell(pos, nodeBounds);
        var posStack = new List<Vector3>(8);
        int stackDepth = 0;

        posStack.Add(pos);
        stackDepth = 1;

        while (true)
        {
            int childID = GetChildID(childCell);
            var childBounds = GetChildBounds(nodeBounds, childCell);

            if (TestChildBit(nodeIndex, childID))
            {
                if (depth == _depth)
                {
                    hitCell = childCell;
                    return true;
                }

                uint childSlot = GetChildSlot(nodeIndex, childID);
                uint childIndex = GetChildIndex(nodeIndex, childSlot);
                Int3 childHit = Int3.Zero;

                if (IntersectTree(pos, dir, invDir, step, childIndex, childBounds, depth + 1, ref childHit))
                {
                    hitCell = (hitCell * 4) + childHit;
                    return true;
                }
            }

            // If the cell is empty or the tree wasn't intersected then walk to next cell.
            var border = childBounds.centre + (Vector3)step * (nodeBounds.extents * 0.25f);
            var s = (border - pos) * invDir;

            if (s.X < s.Y && s.X < s.Z)
            {
                childCell.X += step.X;
                pos += dir * s.X;
                pos.X = border.X;    // Snap to X border for accuracy.
                if (childCell.X < 0 || childCell.X > 3) break;
            }
            else if (s.Y < s.Z)
            {
                childCell.Y += step.Y;
                pos += dir * s.Y;
                pos.Y = border.Y;    // Snap to Y border for accuracy.
                if (childCell.Y < 0 || childCell.Y > 3) break;
            }
            else
            {
                childCell.Z += step.Z;
                pos += dir * s.Z;
                pos.Z = border.Z;    // Snap to Z border for accuracy.
                if (childCell.Z < 0 || childCell.Z > 3) break;
            }
        }

        return false;
    }
}
