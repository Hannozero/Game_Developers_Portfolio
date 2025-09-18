using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region 현재 배틀맵의 종류
    public enum BattleMapType
    {
        Basic, //잡몹전, 모든 적들이 사망시 클리어
        Boss, //보스전, 적캐릭터1(보스) 사망시 클리어
    }

    private BattleMapType battleMapType;
    #endregion

    #region 턴구성
    public enum BattleTurn
    {
        Friendly_StartPhase, //시작페이즈
        Friendly_CharacterSelectPhase,   //아군 캐릭터 선택 페이즈
        Friendly_PageSelectPhase,   //아군 페이지선택
        Friendly_Page_FriendlyTargetingPhase,    //아군 페이지의 대상 선택
        Friendly_Page_EnemyTargetingPhase,    //아군 페이지의 대상 선택
        Friendly_PageActivePhase,   //아군 페이지 진행      
        Friendly_Reasoning1TargetingSelectPhase, //추리스킬1 대상선택
        Friendly_Reasoning2TargetingSelectPhase, //추리스킬2 대상선택
        Friendly_ReasoningActivePhase, //추리스킬 대상선택
        Friendly_EndPhase,   //엔드페이즈
        Enemy_StartPhase,
        Enemy_CharacterSelectPhase,
        Enemy_PageSelectPhase,
        Enemy_PageTargetingPhase,
        Enemy_PageActivePhase,
        Enemy_EndPhase,
        Battle_End, //전투종료!!!

    }
    public BattleTurn battleTurn;
    #endregion

    #region 맵에 있는 캐릭터 
    [Header("아군 캐릭터")]
    [Tooltip("아군캐릭터")] public CharacterBase[] character_Friendly;
    [Header("적군 캐릭터")]
    [Tooltip("적군 캐릭터")] public CharacterBase[] character_Enemy;
    #endregion

    #region 페이지사용시 참조
    [Tooltip("페이지를 사용하는 캐릭터")] public CharacterBase character_Active;
    [Tooltip("타겟이 된 캐릭터")] public CharacterBase character_Target;
    #endregion

    [Header("연동 스크립트")]
    [SerializeField] private BattleUI battleUI;
    [SerializeField] private CameraManager cameraManager;

    [Tooltip("행동횟수")] public int actionNumber = 0;

    #region 필터 on off
    [SerializeField] GameObject filter;
    public void FilterOn()
    {
        filter.SetActive(true);
    }
    public void FilterOff()
    {
        filter.SetActive(false);
    }
    #endregion

    #region 배틀 시 캐릭터 투명도조절
    public void ColorASet(float num)
    {
        foreach (CharacterBase character in character_Friendly)
        {
            if (character == null) continue;
            character.SetSpriteAlpha(num);
        }

        foreach (CharacterBase character in character_Enemy)
        {
            if (character == null) continue;
            character.SetSpriteAlpha(num);
        }
    }
    #endregion


    private void Awake()
    {
        
    }

    private void Start()
    {
        //캐릭터의 테이블 데이터에 따라서 데이터 링크
        for (int i = 0; i < character_Friendly.Length; i++) 
        {
            character_Friendly[i].TableLink();
        }

        for (int i = 0; i < character_Enemy.Length; i++)
        {
            character_Enemy[i].TableLink();
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

        TurnStart();
    }
    #region StartPase에 실행할 함수
    void TurnStart()
    {
        battleTurn = BattleTurn.Friendly_StartPhase;
        #region 잉크게이지 충전
        for (int i = 0; i < character_Friendly.Length; i++)
        {
            character_Friendly[i].character_InkGauge += character_Friendly[i].character_InkGaugeCharge;
            if (character_Friendly[i].character_InkGauge > 10)
            {
                character_Friendly[i].character_InkGauge = 10;
            }
        }
        battleUI.CharacterUiDataUpdate();
        #endregion
        actionNumber = Random.Range(1, 13);
        StartCoroutine(HandRotate());

        #region 적 캐릭터가 사용할 페이지 및 대상 선택
        for (int i = 0; i < character_Enemy.Length; i++)
        {
            character_Active = character_Enemy[i];
            if (character_Enemy[i].livingState == CharacterBase.LivingState.Living)
            {
                //사용스킬 선택
                character_Enemy[i].activePage = character_Enemy[i].character_PageDeck[Random.Range(0, character_Enemy[i].character_PageDeck.Length)];
                Debug.Log(i + "캐릭터가 선택한 페이지" + character_Enemy[i].activePage.page_Name);
                //대상선택
                while (true)
                {
                    character_Target = character_Friendly[Random.Range(0, character_Friendly.Length)];
                    character_Enemy[i].targetCharacter = character_Target;
                    if (character_Target.livingState == CharacterBase.LivingState.Living)
                    {
                        break;
                    }    
                }
                
            }
        }
        #endregion

        #region 버프 턴 카운트 감소
        for (int i = 0; i < character_Friendly.Length; i++)
        {
            character_Friendly[i].BuffTurnDecrease();
        }
        for (int i = 0; i < character_Enemy.Length; i++)
        {
            character_Enemy[i].BuffTurnDecrease();
        }
        #endregion
    }
    #endregion

    //배틀UI함수 CharacterUiDataUpdate호출
    public void UiDataUpdate()
    {
        battleUI.CharacterUiDataUpdate();
    }
    IEnumerator HandRotate()
    {
        for (int i = 0; i < actionNumber; i++)
        {
            battleUI.HandRotateZ(true);
            yield return new WaitForSeconds(0.1f);
        }
        battleTurn = BattleTurn.Friendly_CharacterSelectPhase;
        yield return null;
    }

    public void PageActiveStart()
    {
        actionNumber -= 1;
        battleUI.HandRotateZ(false);
        character_Active.targetCharacter = character_Target;

        //ColorASet(0.1f);
        //character_Active.SetSpriteAlpha(1.0f);
        //character_Target.SetSpriteAlpha(1.0f);

        StartCoroutine(character_Active.PageAnima());
        AllLivingCheck();

        //character_Active.PageActive();
    }
    #region 앤드버튼
    public void EndButton()
    {
        if (battleTurn == BattleTurn.Friendly_CharacterSelectPhase)
            StartCoroutine(Co_EndButton());
    }

    private IEnumerator Co_EndButton()
    {
        battleTurn = BattleTurn.Friendly_EndPhase;

        int running = 0;

        // 아군 버프 동시 시작
        for (int i = 0; i < character_Friendly.Length; i++)
        {
            var ch = character_Friendly[i];
            if (ch == null) continue;
            running++;
            StartCoroutine(CoTrack(ch.BuffActive_EndPhase(), () => running--));
        }

        // 적 버프 동시 시작
        for (int i = 0; i < character_Enemy.Length; i++)
        {
            var ch = character_Enemy[i];
            if (ch == null) continue;
            running++;
            StartCoroutine(CoTrack(ch.BuffActive_EndPhase(), () => running--));
        }

        // 전부 끝날 때까지 대기
        yield return new WaitUntil(() => running == 0);

        battleTurn = BattleTurn.Enemy_StartPhase;
        yield return StartCoroutine(EnemyPlay());
    }

    // 보조: 개별 코루틴이 끝나면 카운터 감소
    private IEnumerator CoTrack(IEnumerator routine, System.Action onDone)
    {
        if (routine != null) yield return routine;
        onDone?.Invoke();
    }
    #endregion
    #region 추리스킬
    public void Reasoning1Button()
    { }
    public void Reasoning2Button()
    {
        if (battleTurn == BattleTurn.Friendly_CharacterSelectPhase)
        {
            if (actionNumber >= 3)
            {
                battleTurn = BattleTurn.Friendly_Reasoning2TargetingSelectPhase;

            }
            else
            { }
        }
    }
    public IEnumerator Reasoning2Action()
    {
        for (int i = 0; i < 3; i++)
        {
            battleUI.HandRotateZ(false);
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }
    #endregion

    #region 적 턴 진행 AI
    public IEnumerator EnemyPlay()
    {
        //턴변경
        battleTurn = BattleTurn.Enemy_StartPhase;

        battleUI.HandRotateClear();
        for (int i = 0; i < character_Enemy.Length; i++)
        {
            battleTurn = BattleTurn.Enemy_CharacterSelectPhase;
            character_Active= character_Enemy[i];
            if (character_Enemy[i].livingState == CharacterBase.LivingState.Living)
            {
                character_Target = character_Enemy[i].targetCharacter;
                character_Enemy[i].targetCharacter = character_Target;

                battleTurn = BattleTurn.Enemy_PageActivePhase;
                if (character_Target.livingState == CharacterBase.LivingState.Living)
                {
                    yield return StartCoroutine(character_Active.PageAnima());
                }
                cameraManager.target_Camera = cameraManager.allView;
            }
            AllLivingCheck();
            yield return new WaitForSeconds(1.0f);
        }

        //아군이 전부 사망해 전투종료상태면 루틴종료
        if (battleTurn == BattleTurn.Battle_End)
        {

        }
        else
        {
            battleTurn = BattleTurn.Enemy_EndPhase;
            TurnStart();
        }

        yield return null;
    }
    #endregion

    #region 생존/사망 상태 갱신 및 전투 종료 체크

    public void AllLivingCheck()
    {
        // 1) 생존/사망 상태 갱신
        for(int i=0;i<character_Friendly.Length;i++) 
        {
            character_Friendly[i].LivingStateCheck();
        }

        for (int i = 0; i < character_Enemy.Length; i++)
        {
            character_Enemy[i].LivingStateCheck();
        }


        // 2) UI 갱신
        battleUI.CharacterUiDataUpdate();

        // 3) 전멸/사망 판정용 플래그 계산
        bool allEnemiesDead = true;
        for (int i = 0; i < character_Enemy.Length; i++)
        {
            if (character_Enemy[i].livingState != CharacterBase.LivingState.Die)
            {
                allEnemiesDead = false;
                break;
            }
        }

        bool allFriendliesDead = true;
        for (int i = 0; i < character_Friendly.Length; i++)
        {
            if (character_Friendly[i].livingState != CharacterBase.LivingState.Die)
            {
                allFriendliesDead = false;
                break;
            }
        }

        // 4) 맵 타입별 승리 조건 체크
        switch (battleMapType)
        {
            case BattleMapType.Basic:
                // 기본 맵: 모든 적 사망 시 종료
                if (allEnemiesDead)
                {
                    Debug.Log("모든 적 사망확인! 전투종료!");
                    EndBattle_Win();
                    return;
                }
                break;

            case BattleMapType.Boss:
                // 보스 맵: 보스(0번) 사망 시 종료
                if (character_Enemy.Length > 0 &&
                    character_Enemy[0].livingState == CharacterBase.LivingState.Die)
                {
                    Debug.Log("보스 사망확인! 전투종료!");
                    EndBattle_Win();
                    return;
                }
                break;
        }

        // 5) 패배 조건(아군 전멸) 체크
        if (allFriendliesDead)
        {
            Debug.Log("아군 전멸! 전투종료!");
            EndBattle_Lose();
            return;
        }

    }
    #endregion

    #region 전투종료

    private void EndBattle_Win()
    {
        battleTurn = BattleTurn.Battle_End;
        battleUI.BattleEndUIOn();
    }
    private void EndBattle_Lose()
    {
        battleTurn = BattleTurn.Battle_End;
        battleUI.BattleEndUIOn();
    }
    #endregion
}
