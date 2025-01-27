using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] GameObject hazardPrefab;

    private GameObject character;
    private GameObject target;
    private GameObject hazard;
    private TankController tankController;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartSeek();
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartFlee();
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartArrive();
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartAvoid();
        if (Input.GetKeyDown(KeyCode.Alpha0)) ResetScene();
    }

    private void StartSeek()
    {
        SetupCharacterAndTarget();
        tankController.StartSeek();
    }

    private void StartFlee()
    {
        SetupCharacterAndTarget();
        tankController.StartFlee();
    }

    private void StartArrive()
    {
        SetupCharacterAndTarget();
        tankController.StartArrive();
    }

    private void StartAvoid()
    {
        SetupCharacterAndTarget();
        // Spawn hazard along the path
        Vector3 hazardPosition = Vector3.Lerp(character.transform.position, target.transform.position, Random.Range(0.3f, 0.7f));
        hazard = Instantiate(hazardPrefab, hazardPosition, Quaternion.identity);

        tankController.hazard = hazard.transform;
        tankController.StartAvoid();
    }

    private void ResetScene()
    {
        if (character != null) Destroy(character);
        if (target != null) Destroy(target);
        if (hazard != null) Destroy(hazard);
    }

    private void SetupCharacterAndTarget()
    {
        ResetScene();
        character = Instantiate(characterPrefab, GetRandomPosition(), Quaternion.identity);
        target = Instantiate(targetPrefab, GetRandomPosition(), Quaternion.identity);
        tankController = character.GetComponent<TankController>();
        tankController.target = target.transform;
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f),0);
    }
}
