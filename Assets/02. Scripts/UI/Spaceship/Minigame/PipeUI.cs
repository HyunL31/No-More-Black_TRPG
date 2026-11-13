using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PipeDirection
{
    None = 0,
    Up = 1,
    Right = 2,
    Down = 4,
    Left = 8
}

public class PipeUI : ViewBase
{
    [SerializeField] private Pipe Prefab_Straight;
    [SerializeField] private Pipe Prefab_Corner;
    [SerializeField] private Pipe Prefab_T;
    [SerializeField] private Pipe Prefab_Cross;
    [SerializeField] private float _spacing = 100f;
    [SerializeField] private Button Button_Confirm;

    [SerializeField] private Timer Timer;
    [SerializeField] private int Timer_Limit;

    private int _width = 8;
    private int _height = 4;

    private Vector3 _origin;

    private Vector2Int _start = new Vector2Int(0, 0);
    private Vector2Int _end = new Vector2Int(7, 3);

    public bool IsClear { get; private set; }

    private PipeDirection[,] _map;
    private Pipe[,] _pipes;

    private void Awake()
    {
        Button_Confirm.onClick.AddListener(OnClickConfirm);
    }

    private void Start()
    {
        GenerateMap();
    }

    private void OnEnable()
    {
        Timer.OnTimeOver += OnClickConfirm;
        Timer.StartTimer(Timer_Limit);
    }

    private void OnDisable()
    {
        Timer.OnTimeOver -= OnClickConfirm;
    }

    private void OnClickConfirm()
    {
        if (IsClear)
        {
            return;
        }

        if (CheckConnection() == true)
        {
            IsClear = true;
            UIManager.Inst.OpenWinPanel();
        }
        else
        {
            UIManager.Inst.OpenFailPanel();
        }
    }

    private void GenerateMap()
    {
        if (IsInside(_start) == false || IsInside(_end) == false || _start == _end)
        {
            return;
        }

        if (_start.x != 0 || _end.x != _width - 1)
        {
            return;
        }

        if (_pipes != null)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_pipes[x, y] != null)
                    {
                        Destroy(_pipes[x, y].gameObject);
                    }
                }
            }
        }

        IsClear = false;

        _map = new PipeDirection[_width, _height];
        _pipes = new Pipe[_width, _height];

        GenerateMaze();
        AddStartEndConnection();
        EnsureMinimumConnection();

        if (ValidateAnswerMap() == false)
        {
            return;
        }

        SetOrigin();
        CreatePipes();
    }

    private void GenerateMaze()
    {
        bool[,] visited = new bool[_width, _height];

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        Vector2Int first = new Vector2Int(Random.Range(0, _width), Random.Range(0, _height));

        visited[first.x, first.y] = true;
        stack.Push(first);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();

            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current, visited);

            if (neighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];

            Connect(current, next);

            visited[next.x, next.y] = true;
            stack.Push(next);
        }
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int current, bool[,] visited)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int next = current + direction;

            if (IsInside(next) == false)
            {
                continue;
            }

            if (visited[next.x, next.y] == false)
            {
                result.Add(next);
            }
        }

        return result;
    }

    private void Connect(Vector2Int a, Vector2Int b)
    {
        Vector2Int dir = b - a;

        PipeDirection aDir = GetDirection(dir);
        PipeDirection bDir = GetDirection(-dir);

        _map[a.x, a.y] |= aDir;
        _map[b.x, b.y] |= bDir;
    }

    private void AddStartEndConnection()
    {
        _map[_start.x, _start.y] |= PipeDirection.Left;
        _map[_end.x, _end.y] |= PipeDirection.Right;
    }

    private void EnsureMinimumConnection()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2Int current = new Vector2Int(x, y);

                if (CountConnections(_map[x, y]) >= 2)
                {
                    continue;
                }

                List<Vector2Int> candidates = GetAvailableNeighbors(current);

                if (candidates.Count == 0)
                {
                    continue;
                }

                Vector2Int next = candidates[Random.Range(0, candidates.Count)];

                Connect(current, next);
            }
        }
    }

    private List<Vector2Int> GetAvailableNeighbors(Vector2Int current)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int next = current + dir;

            if (IsInside(next) == false)
            {
                continue;
            }

            PipeDirection currentDir = GetDirection(dir);

            if ((_map[current.x, current.y] & currentDir) == 0)
            {
                result.Add(next);
            }
        }

        return result;
    }

    private bool ValidateAnswerMap()
    {
        bool[,] visited = new bool[_width, _height];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(_start);
        visited[_start.x, _start.y] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == _end)
            {
                return (_map[_end.x, _end.y] & PipeDirection.Right) != 0;
            }

            Vector2Int[] directions = { Vector2Int.up,  Vector2Int.right, Vector2Int.down, Vector2Int.left };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (IsInside(next) == false)
                {
                    continue;
                }

                if (visited[next.x, next.y])
                {
                    continue;
                }

                PipeDirection currentDir = GetDirection(dir);
                PipeDirection nextDir = GetDirection(-dir);

                bool connected = (_map[current.x, current.y] & currentDir) != 0 && (_map[next.x, next.y] & nextDir) != 0;

                if (connected == false)
                {
                    continue;
                }

                visited[next.x, next.y] = true;
                queue.Enqueue(next);
            }
        }

        return false;
    }

    private void CreatePipes()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                PipeDirection answer = _map[x, y];

                Pipe prefab = GetPrefab(answer);

                if (prefab == null)
                {
                    return;
                }

                Pipe pipe = Instantiate(prefab, transform);

                pipe.transform.localPosition = _origin + new Vector3( x * _spacing, y * _spacing, 0f);

                _pipes[x, y] = pipe;

                int correctStep = GetRotationStep(GetBaseDirection(prefab), answer);
                int randomStep = Random.Range(0, 4);
                bool canRotate = new Vector2Int(x, y) != _start && new Vector2Int(x, y) != _end;

                pipe.Initialize(this, answer, correctStep, randomStep, canRotate);
            }
        }
    }

    private Pipe GetPrefab(PipeDirection direction)
    {
        int count = CountConnections(direction);

        if (count == 2)
        {
            bool straight = direction == (PipeDirection.Up | PipeDirection.Down) || direction == (PipeDirection.Left | PipeDirection.Right);

            return straight ? Prefab_Straight : Prefab_Corner;
        }

        if (count == 3)
        {
            return Prefab_T;
        }

        if (count == 4)
        {
            return Prefab_Cross;
        }

        return null;
    }

    public bool CheckConnection()
    {
        if (_pipes == null)
        {
            return false;
        }

        bool[,] visited = new bool[_width, _height];

        Pipe startPipe = _pipes[_start.x, _start.y];

        if (startPipe == null)
        {
            return false;
        }

        if ((startPipe.CurrentDirection & PipeDirection.Left) == 0)
        {
            return false;
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(_start);
        visited[_start.x, _start.y] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            Pipe currentPipe = _pipes[current.x, current.y];

            if (currentPipe == null)
            {
                continue;
            }

            if (current == _end)
            {
                return (currentPipe.CurrentDirection & PipeDirection.Right) != 0;
            }

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (IsInside(next) == false)
                {
                    continue;
                }

                if (visited[next.x, next.y])
                {
                    continue;
                }

                Pipe nextPipe = _pipes[next.x, next.y];

                if (nextPipe == null)
                {
                    continue;
                }

                PipeDirection currentDir = GetDirection(dir);
                PipeDirection nextDir = GetDirection(-dir);

                bool currentConnected = (currentPipe.CurrentDirection & currentDir) != 0;

                bool nextConnected = (nextPipe.CurrentDirection & nextDir) != 0;

                if (currentConnected == false || nextConnected == false)
                {
                    continue;
                }

                visited[next.x, next.y] = true;
                queue.Enqueue(next);
            }
        }

        return false;
    }

    private PipeDirection GetBaseDirection(Pipe prefab)
    {
        if (prefab == Prefab_Straight)
        {
            return PipeDirection.Up | PipeDirection.Down;
        }

        if (prefab == Prefab_Corner)
        {
            return PipeDirection.Up | PipeDirection.Right;
        }

        if (prefab == Prefab_T)
        {
            return PipeDirection.Up |
                   PipeDirection.Right |
                   PipeDirection.Left;
        }

        if (prefab == Prefab_Cross)
        {
            return PipeDirection.Up |
                   PipeDirection.Right |
                   PipeDirection.Down |
                   PipeDirection.Left;
        }

        return PipeDirection.None;
    }

    private int GetRotationStep(PipeDirection baseDirection, PipeDirection answer)
    {
        PipeDirection direction = baseDirection;

        for (int i = 0; i < 4; i++)
        {
            if (direction == answer)
            {
                return i;
            }

            direction = RotateClockwise(direction);
        }

        return 0;
    }

    private PipeDirection RotateClockwise(PipeDirection direction)
    {
        PipeDirection result = PipeDirection.None;

        if ((direction & PipeDirection.Up) != 0)
        {
            result |= PipeDirection.Right;
        }

        if ((direction & PipeDirection.Right) != 0)
        {
            result |= PipeDirection.Down;
        }

        if ((direction & PipeDirection.Down) != 0)
        {
            result |= PipeDirection.Left;
        }

        if ((direction & PipeDirection.Left) != 0)
        {
            result |= PipeDirection.Up;
        }

        return result;
    }

    private PipeDirection GetDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.up)
        {
            return PipeDirection.Up;
        }

        if (dir == Vector2Int.right)
        {
            return PipeDirection.Right;
        }

        if (dir == Vector2Int.down)
        {
            return PipeDirection.Down;
        }

        if (dir == Vector2Int.left)
        {
            return PipeDirection.Left;
        }

        return PipeDirection.None;
    }

    private int CountConnections(PipeDirection direction)
    {
        int count = 0;

        if ((direction & PipeDirection.Up) != 0) count++;
        if ((direction & PipeDirection.Right) != 0) count++;
        if ((direction & PipeDirection.Down) != 0) count++;
        if ((direction & PipeDirection.Left) != 0) count++;

        return count;
    }

    private bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
    }

    private void SetOrigin()
    {
        float totalWidth = (_width - 1) * _spacing;
        float totalHeight = (_height - 1) * _spacing;

        _origin = new Vector3(-totalWidth / 2f, -totalHeight / 2f, 0f);
    }
}