using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{

    [Header("캐릭터베이스 연동")]
    [SerializeField, Tooltip("캐릭터베이스")] private CharacterBase characterBase;

    [Header("연동 스크립트")]
    [Tooltip("배틀 시스템")] private BattleSystem battleSystem;
    [Tooltip("카메라 매니저")] CameraManager cameraManager;

    [Header("표식")]
    [SerializeField, Tooltip("베이직 표식")] public GameObject basicMark;
    [SerializeField, Tooltip("타켓으로 지정하기 위해 마우스를 올리면 바뀔 표식")] public GameObject targetMark;

    private void Start()
    {
        // 배틀시스템 찾기
        if (battleSystem == null)
        {
            battleSystem = FindFirstObjectByType<BattleSystem>();
            if (battleSystem == null)
            {
                Debug.LogError("BattleSystem을 씬에서 찾을 수 없습니다.");
            }
        }

        // 카메라 매니저 찾기
        if (cameraManager == null)
        {
            cameraManager = FindFirstObjectByType<CameraManager>();
            if (cameraManager == null)
            {
                Debug.LogError("CameraManager를 씬에서 찾을 수 없습니다.");
            }
        }
    }


    #region 마우스트리거
    #region 마우스를 올리면
    private void OnMouseEnter()
    {
        if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_Page_EnemyTargetingPhase)
        {
            targetMark.SetActive(true);
        }
    }
    #endregion
    #region 마우스를 때면
    private void OnMouseExit()
    {
        basicMark.SetActive(false);
        targetMark.SetActive(false);
    }
    #endregion
    #region 마우스로 클릭하면
    public void OnMouseDown()
    {
        switch (battleSystem.battleTurn)
        {
            case BattleSystem.BattleTurn.Friendly_Page_EnemyTargetingPhase:
                if (characterBase.livingState == CharacterBase.LivingState.Living)
                {
                    battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_PageActivePhase;
                    battleSystem.character_Target = characterBase;
                    battleSystem.PageActiveStart();
                }
                break;
        }

    }
    #endregion
    #endregion
}
