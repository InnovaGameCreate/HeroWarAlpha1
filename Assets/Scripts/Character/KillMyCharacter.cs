using Fusion;
using Unit;
using UnityEngine;

public class KillMyCharacter : NetworkBehaviour
{
    private CharacterProfile _target;
    
    void Start()
    {
        _target = GetComponent<CharacterProfile>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            if (Object.HasInputAuthority && _target.GetCharacterState() != CharacterState.Dead)
            {
                _target.AddDamage(999f);
                Debug.Log("999ダメージ！");
            }
        }
    }
}
