using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterManagement : MonoBehaviour
{
    // Player tracking and management
    public List<GameObject> _playableCharacters;
    public GameObject activeCharacter;
    private bool _followActivePlayer = false;

    #region Singleton
    public static CharacterManagement Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }
    #endregion

    void Update()
    {
        if (Input.GetButtonDown("Ember"))
        {
           ActivateCharacterAtIndex(0);
        }
        else if (Input.GetButtonDown("Ezra"))
        {
            ActivateCharacterAtIndex(1);
        }
        else if (Input.GetButtonDown("Adam"))
        {
            ActivateCharacterAtIndex(2);
        }
    }

    void ActivateCharacterAtIndex(int i)
    {
        if (CheckIfCharacterIsAlive(_playableCharacters[i]) && CheckForNewCharDifferent(_playableCharacters[i]))
        {
            SetControls(true);
            activeCharacter = _playableCharacters[i];
            SetControls(false);
        }
    }

    public void SetFollowActivePlayer()
    {
        _followActivePlayer = !_followActivePlayer;
        for (int i = 0; i < _playableCharacters.Count; i++)
        {
            _playableCharacters[i].GetComponent<PlayerController>().followPlayer = _followActivePlayer;
            SetCrouchForFollowingCharacters(_followActivePlayer);
        }  
    }

    public void SetCrouchForFollowingCharacters(bool _b)
    {
        for (int i = 0; i < _playableCharacters.Count; i++)
            if (!_playableCharacters[i].GetComponent<PlayerController>().isActiveCharacter &&
                _playableCharacters[i].GetComponent<PlayerController>().followPlayer)
                _playableCharacters[i].GetComponent<PlayerController>().crouching = _b; 
    }

    public void RefillSkillPointsForAllCharacters()
    {
        for (int i = 0; i < _playableCharacters.Count; i++)
        {
            PlayerController _pc = _playableCharacters[i].GetComponent<PlayerController>();
            _pc.skillPoints = _pc.maximumSkillPoints;
        }
    }

    int GetActiveIndex()
    {
        for (int i = 0; i < _playableCharacters.Count; i++)
            if (activeCharacter.name == _playableCharacters[i].name)
                return i;
        return 3;
    }

    void SetControls(bool b)
    {
        activeCharacter.GetComponent<PlayerController>().isActiveCharacter = !b;
        activeCharacter.GetComponent<NavMeshAgent>().enabled = b;
    }

    bool CheckForNewCharDifferent(GameObject _newChar)
    {
        if (_newChar != activeCharacter)
        {
            return true;
        }
        return false;
    }

    bool CheckIfCharacterIsAlive(GameObject _newChar)
    {
        if (_newChar.GetComponent<Character>().healthPoints > 0)
        {
            return true;
        }
        else return false;
    }

    public UIInformationObj ReturnActiveCharInformation()
    {
        UIInformationObj _information = new UIInformationObj();
        PlayerController _c = activeCharacter.GetComponent<PlayerController>();
        _information.health = _c.healthPoints;
        _information.name = _c.gameObject.name;
        _information.surveilanceLevel = (int)Mathf.Round(_c.surveilanceLevel);
        _information.skillAvailable = _c.skillAvailable;
        _information.information = _c.information;
        _information.index = GetActiveIndex();
        return _information;
    }
}
