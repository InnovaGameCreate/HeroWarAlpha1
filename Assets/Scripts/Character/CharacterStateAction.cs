using Fusion;

public abstract class CharacterStateAction : NetworkBehaviour
{
    public abstract void StartStateAction();
    public abstract void StateAction();
    public abstract void EndStateAction();
}
