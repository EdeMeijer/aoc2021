namespace Aoc2021;

public class Day23
{
    public static long Part1() =>
        Solve(new []
        {
            new []{ 1, 2 },
            new []{ 1, 2 },
            new []{ 3, 0 },
            new []{ 3, 0 },
        });

    public static long Part2() =>
        Solve(new []
        {
            new []{ 1, 3, 3, 2 },
            new []{ 1, 2, 1, 2 },
            new []{ 3, 1, 0, 0 },
            new []{ 3, 0, 2, 0 },
        });
    
    private static long Solve(int[][] amphipods)
    {
        var rooms = new Room[]
        {
            new(2, amphipods[0]),
            new(4, amphipods[1]),
            new(6, amphipods[2]),
            new(8, amphipods[3])
        };

        var hallway = new HallNode[]
        {
            new(),
            new(),
            new(0),
            new(),
            new(1),
            new(),
            new(2),
            new(),
            new(3),
            new(),
            new()
        };

        var history = new Stack<Move>();
        var moveCounter = new List<long>();
        var score = 0L;
        var bestScore = long.MaxValue;

        Move? revert = null;
        for (var step = 0;; step ++)
        {
            if (revert != null)
            {
                score -= Rollback(rooms, hallway, revert);
            }

            var nextMove = GetNextMove(rooms, hallway, revert);
            if (nextMove == null)
            {
                // No possible moves left
                if (IsSolution(rooms) && score < bestScore)
                {
                    Console.WriteLine($"=== better solution ({score})");
                    bestScore = score;
                }

                if (!history.Any())
                {
                    // All possible scenario's processed
                    break;
                }

                // Remove the last move from the stack and try something else instead
                revert = history.Pop();
                continue;
            }

            if (moveCounter.Count == history.Count)
            {
                moveCounter.Add(0);
            }

            moveCounter[history.Count] ++;

            revert = null;

            score += Apply(rooms, hallway, nextMove);

            if (score >= bestScore)
            {
                // No need to look further
                revert = nextMove;
            }
            else
            {
                history.Push(nextMove);
            }

            if (step % 1000000 == 0)
            {
                // Debug output for getting a sense of progress
                // Console.WriteLine(string.Join(" - ", moveCounter));
            }
        }

        return bestScore;
    }

    private static Move? GetNextMove(Room[] rooms, HallNode[] hallway, Move? prevMove)
    {
        if (prevMove is not GotoRoom)
        {
            var prevHallwayMove = prevMove as GotoHallway;
            var startRoom = prevHallwayMove?.fromRoomIndex ?? 0;

            for (var roomIndex = startRoom; roomIndex < rooms.Length; roomIndex ++)
            {
                var room = rooms[roomIndex];
                if (room.Amphipods.Any(amphipod => amphipod != roomIndex))
                {
                    var continuingMove = prevHallwayMove != null && roomIndex == startRoom ? prevHallwayMove : null;

                    // Consider moving to the left in the hallway
                    if (continuingMove == null || continuingMove.toNodeIndex < room.HallNode)
                    {
                        var nodeIndex = continuingMove?.toNodeIndex ?? room.HallNode;
                        nodeIndex --;
                        if (nodeIndex > 0 && hallway[nodeIndex].Room != null)
                        {
                            nodeIndex --;
                        }

                        if (nodeIndex >= 0 && PathIsClear(hallway, room.HallNode, nodeIndex))
                        {
                            return new GotoHallway(nodeIndex, roomIndex);
                        }
                    }

                    // Consider moving to the right
                    {
                        var nodeIndex = continuingMove?.toNodeIndex > room.HallNode
                            ? continuingMove.toNodeIndex
                            : room.HallNode;

                        nodeIndex ++;
                        if (nodeIndex < hallway.Length && hallway[nodeIndex].Room != null)
                        {
                            nodeIndex ++;
                        }

                        if (nodeIndex < hallway.Length && PathIsClear(hallway, room.HallNode, nodeIndex))
                        {
                            return new GotoHallway(nodeIndex, roomIndex);
                        }
                    }
                }
            }
        }

        // Cannot move any amphipod out of a room. Now try moving them from the hall into a room
        var prevRoomMove = prevMove as GotoRoom;
        var startNode = prevRoomMove?.fromNodeIndex + 1 ?? 0;
        for (var nodeIndex = startNode; nodeIndex < hallway.Length; nodeIndex ++)
        {
            var node = hallway[nodeIndex];
            if (node.Amphipod != null)
            {
                var roomIndex = node.Amphipod.Value;
                // It can only go into it's corresponding room, and only if the path to that room is clear and no
                // other types are present in the room
                var room = rooms[roomIndex];
                var scanOffset = room.HallNode < nodeIndex ? -1 : 1;
                if (!PathIsClear(hallway, nodeIndex + scanOffset, room.HallNode))
                {
                    continue;
                }

                if (room.Amphipods.Any(inRoom => inRoom != roomIndex))
                {
                    // Some other type still in room
                    continue;
                }

                // Free to go
                return new GotoRoom(roomIndex, nodeIndex);
            }
        }

        return null;
    }

    private static readonly long[] energyUse = { 1, 10, 100, 1000 };

    private static long Apply(Room[] rooms, HallNode[] hallway, Move move)
    {
        if (move is GotoHallway toHallway)
        {
            var room = rooms[toHallway.fromRoomIndex];
            var amphipod = room.PopAmphipod();
            hallway[toHallway.toNodeIndex].PushAmphipod(amphipod);

            var vSteps = room.Capacity - room.Amphipods.Length;
            var hSteps = Math.Abs(room.HallNode - toHallway.toNodeIndex);
            return (vSteps + hSteps) * energyUse[amphipod];
        }

        if (move is GotoRoom toRoom)
        {
            var room = rooms[toRoom.toRoomIndex];
            var vSteps = room.Capacity - room.Amphipods.Length;

            var amphipod = hallway[toRoom.fromNodeIndex].PopAmphipod();
            room.PushAmphipod(amphipod);

            var hSteps = Math.Abs(room.HallNode - toRoom.fromNodeIndex);
            return (vSteps + hSteps) * energyUse[amphipod];
        }

        throw new Exception();
    }

    private static long Rollback(Room[] rooms, HallNode[] hallway, Move move)
    {
        Move reverse = move switch
        {
            GotoRoom toRoom => new GotoHallway(toRoom.fromNodeIndex, toRoom.toRoomIndex),
            GotoHallway toHallway => new GotoRoom(toHallway.fromRoomIndex, toHallway.toNodeIndex)
        };

        return Apply(rooms, hallway, reverse);
    }

    private static bool PathIsClear(HallNode[] hallway, int nodeA, int nodeB)
    {
        var (from, to) = (Math.Min(nodeA, nodeB), Math.Max(nodeA, nodeB));
        for (var i = from; i <= to; i ++)
        {
            if (hallway[i].Amphipod != null)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsSolution(Room[] rooms)
    {
        for (var i = 0; i < rooms.Length; i ++)
        {
            var room = rooms[i];
            if (room.Amphipods.Length != room.Capacity || room.Amphipods.Any(amphipod => amphipod != i))
            {
                return false;
            }
        }

        return true;
    }

    private abstract record Move;

    private sealed record GotoHallway(int toNodeIndex, int fromRoomIndex) : Move;

    private sealed record GotoRoom(int toRoomIndex, int fromNodeIndex) : Move;

    private sealed class Room
    {
        public int HallNode { get; }
        public int Capacity { get; }
        public int[] Amphipods { get; private set; }

        public Room(int hallNode, params int[] amphipods)
        {
            HallNode = hallNode;
            Capacity = amphipods.Length;
            Amphipods = amphipods;
        }

        public int PopAmphipod()
        {
            if (!Amphipods.Any())
            {
                throw new Exception();
            }

            var result = Amphipods[0];
            Amphipods = Amphipods[1..];
            return result;
        }

        public void PushAmphipod(int amphipod)
        {
            if (Amphipods.Length == Capacity)
            {
                throw new Exception();
            }

            Amphipods = new[] { amphipod }.Concat(Amphipods).ToArray();
        }
    }

    private sealed class HallNode
    {
        public int? Room { get; }
        public int? Amphipod { get; private set; }

        public HallNode(int? room = null)
        {
            Room = room;
        }

        public int PopAmphipod()
        {
            if (Amphipod == null)
            {
                throw new Exception();
            }

            var result = Amphipod.Value;
            Amphipod = null;
            return result;
        }

        public void PushAmphipod(int amphipod)
        {
            if (Amphipod != null)
            {
                throw new Exception();
            }

            Amphipod = amphipod;
        }
    }
}
