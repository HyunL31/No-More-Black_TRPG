using UnityEngine;

public enum SpawnType
{
    None,
    PersonalEvent
}

public class SpawnSpot : MonoBehaviour
{
    [SerializeField] private SpawnType SpawnType_Type;
    [SerializeField] private string String_SpawnID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartSpawn();
        }
    }

    private void StartSpawn()
    {
        switch (SpawnType_Type)
        {
            case SpawnType.PersonalEvent:
                StartPersonalEvent(String_SpawnID);
                break;
        }
    }

    private void StartPersonalEvent(string characterID)
    {
        if (SaveManager.Inst.PlayerModel.PersonalEvent.ContainsKey(characterID) == true && SaveManager.Inst.PlayerModel.PersonalEvent[characterID].Count >= 3)
        {
            return;
        }

        int random;

        while (true)
        {
            random = Random.Range(1, 4);

            if (SaveManager.Inst.PlayerModel.PersonalEvent.ContainsKey(characterID) == false || SaveManager.Inst.PlayerModel.PersonalEvent[characterID].Contains(random) == false)
            {
                break;
            }
        }

        string textID = $"{characterID}_0{random}_01";

        TextAdventureManager.Inst.SetCurrentID(textID);

        UIManager.Inst.OpenBackground();
        UIManager.Inst.OpenPersonalEventUI(characterID, random);
    }
}