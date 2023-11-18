using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayerLogic;
using UnityEngine;

public class CharacterMenu : MonoBehaviour
{
    private bool _hasCharacter = false;

    private IPlayer.PlayerCharacter _selectedCharacter;

    public void SelectTank()
    {
        _selectedCharacter = IPlayer.PlayerCharacter.Tank;
        _hasCharacter = true;
    }

    public void SelectArcher()
    {
        _selectedCharacter = IPlayer.PlayerCharacter.Archer;
        _hasCharacter = true;
    }

    public async Task<IPlayer.PlayerCharacter> SelectCharacter()
    {
        while (!_hasCharacter)
            await Task.Yield();

        gameObject.SetActive(false);

        return _selectedCharacter;
    }
}
