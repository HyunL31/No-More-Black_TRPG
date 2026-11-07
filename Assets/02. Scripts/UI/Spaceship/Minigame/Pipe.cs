using UnityEngine;
using UnityEngine.UI;

public class Pipe : MonoBehaviour
{
    [SerializeField] private Button Button_Pipe;

    public PipeDirection CurrentDirection { get; private set; }

    private PipeUI _manager;
    private int _rotationStep;
    private bool _canRotate;

    public void Initialize(PipeUI manager, PipeDirection answer, int correctStep, int randomStep, bool canRotate = true)
    {
        _manager = manager;
        _canRotate = canRotate;
        CurrentDirection = answer;

        for (int i = 0; i < randomStep; i++)
        {
            CurrentDirection = RotateClockwise(CurrentDirection);
        }

        _rotationStep = (correctStep + randomStep) % 4;

        transform.localRotation = Quaternion.Euler(0f, 0f, -90f * _rotationStep);

        if (Button_Pipe != null)
        {
            Button_Pipe.onClick.RemoveListener(OnClickPipe);
            Button_Pipe.onClick.AddListener(OnClickPipe);
        }
    }

    private void OnClickPipe()
    {
        if (_manager == null || _manager.IsClear || _canRotate == false)
        {
            return;
        }

        _rotationStep = (_rotationStep + 1) % 4;

        CurrentDirection = RotateClockwise(CurrentDirection);

        transform.localRotation = Quaternion.Euler(0f, 0f, -90f * _rotationStep);
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
}