using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyCharacter : MonoBehaviour
{
    [Header("캐릭터베이스 연동")]
    [SerializeField, Tooltip("캐릭터베이스")] private CharacterBase characterBase;

    [Header("연동 스크립트")]
    [Tooltip("배틀 시스템")] private BattleSystem battleSystem;
    [Tooltip("카메라 매니저")] CameraManager cameraManager;

    [Header("UI")]
    [SerializeField, Tooltip("전체 UI")] GameObject character_ContainerUI;
    [SerializeField, Tooltip("취소버튼")] GameObject character_CancelButton;
    [SerializeField, Tooltip("페이지 UI")] GameObject character_PageUI;
    [SerializeField, Tooltip("페이지 이미지")] Image character_PageImage;
    [SerializeField, Tooltip("페이지 이름")] TMP_Text character_PageName;
    [SerializeField, Tooltip("페이지 잉크소모량")] TMP_Text character_PageInk;
    [SerializeField, Tooltip("페이지 설명")] TMP_Text character_PageExplain;

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

    #region UI 관련 함수 및 코루틴

    /// <summary>
    /// 페이지데이터 출력
    /// </summary>
    void PageDataPrint()
    {
        character_PageImage.sprite = characterBase.character_PageDeck[pagePrintNumber].page_Image;
        character_PageName.text = characterBase.character_PageDeck[pagePrintNumber].page_Name;
        character_PageInk.text = characterBase.character_PageDeck[pagePrintNumber].page_InkGauge.ToString();
        character_PageExplain.text = characterBase.character_PageDeck[pagePrintNumber].page_Explanation;
    }

    [SerializeField, Tooltip("현재 표시중인 페이지번호")] int pagePrintNumber = 0;
    public void NextPage(int num)
    {
        int targetIndex = pagePrintNumber + num;

        // 배열 범위 초과 방지 (optional)
        if (targetIndex < 0 || targetIndex >= characterBase.character_PageDeck.Length)
        {
            Debug.LogWarning("선택한 인덱스가 범위를 벗어났습니다.");
            return;
        }

        if (characterBase.character_PageDeck[targetIndex] != null)
        {
            Debug.Log("null이 아닙니다! 페이지 출력합니다.");

            pagePrintNumber = targetIndex;
            PageDataPrint();
        }
        else
        {
            Debug.Log("페이지가 null입니다. 출력하지 않습니다.");
        }

    }

    //사용할 페이지 클릭
    public void PageButtonDown()
    {
        switch (battleSystem.battleTurn)
        {
            case BattleSystem.BattleTurn.Friendly_PageSelectPhase:
                if (characterBase.character_InkGauge >= characterBase.character_PageDeck[pagePrintNumber].page_InkGauge && battleSystem.actionNumber > 0)
                {
                    if (characterBase.character_PageDeck[pagePrintNumber].pageActiveType == PageTable.PageActiveType.Single_Attack)
                    {
                        battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_Page_EnemyTargetingPhase;
                        battleSystem.character_Active = characterBase;
                        characterBase.activePage = characterBase.character_PageDeck[pagePrintNumber];
                    }
                    else if (characterBase.character_PageDeck[pagePrintNumber].pageActiveType == PageTable.PageActiveType.Single_Heal || characterBase.character_PageDeck[pagePrintNumber].pageActiveType == PageTable.PageActiveType.Single_Shield)
                    {
                        battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_Page_FriendlyTargetingPhase;
                        battleSystem.character_Active = characterBase;
                        characterBase.activePage = characterBase.character_PageDeck[pagePrintNumber];
                    }
                    else if (characterBase.character_PageDeck[pagePrintNumber].pageActiveType == PageTable.PageActiveType.Aoe_Attack || characterBase.character_PageDeck[pagePrintNumber].pageActiveType == PageTable.PageActiveType.Aoe_Heal || characterBase.character_PageDeck[pagePrintNumber].pageActiveType == PageTable.PageActiveType.Aoe_Shield)
                    {
                        battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_PageActivePhase;
                        battleSystem.character_Active = characterBase;
                        characterBase.activePage = characterBase.character_PageDeck[pagePrintNumber];
                        battleSystem.PageActiveStart();
                    }
                }
                else
                {
                    battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_CharacterSelectPhase;
                }
                    character_ContainerUI.SetActive(false);          
                break;
        }
        
    }

    //취소버튼 클릭
    public void CancleButtonDown()
    {

        switch (battleSystem.battleTurn)
        {
            case BattleSystem.BattleTurn.Friendly_PageSelectPhase:
                battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_CharacterSelectPhase;
                character_ContainerUI.SetActive(false);
                break;
            case BattleSystem.BattleTurn.Friendly_Page_FriendlyTargetingPhase:
                battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_CharacterSelectPhase;
                character_ContainerUI.SetActive(false);
                break;
        }
    }
    #endregion
    #region 마우스트리거
    #region 마우스를 올리면
    private void OnMouseEnter()
    {
        if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_CharacterSelectPhase)
        {
            basicMark.SetActive(true);
        }
        else if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_Page_FriendlyTargetingPhase)
        {
            targetMark.SetActive(true);
        }
        else if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_Reasoning2TargetingSelectPhase)
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
        if (characterBase.livingState == CharacterBase.LivingState.Living)
        {
            if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_CharacterSelectPhase)
            {
                battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_PageSelectPhase;
                cameraManager.target_Camera = characterBase.character_SelectView;
                pagePrintNumber = 0;
                character_ContainerUI.SetActive(true);
                character_PageUI.SetActive(true);
                PageDataPrint();
                Debug.Log("UI오픈");
            }
            else if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_Page_FriendlyTargetingPhase)
            {
                battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_PageActivePhase;
                battleSystem.character_Target = characterBase;
                battleSystem.PageActiveStart();
            }
            else if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_Reasoning2TargetingSelectPhase)
            {
                battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_ReasoningActivePhase;
                battleSystem.actionNumber -= 3;
                StartCoroutine(battleSystem.Reasoning2Action());
                characterBase.character_InkGauge += 3;
                battleSystem.UiDataUpdate();
                battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_CharacterSelectPhase;

            }


        }
    }
    #endregion
    #endregion
}
