using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary> [스크립트 설명]
/// 게임캐릭터의 기반이 되는 베이스 스크립트입니다.
/// 작업자 : 전재연
/// </summary>

public class CharacterBase : MonoBehaviour
{
    [Header("캐릭터 정보")]
    [Tooltip("캐릭터 테이블")] public CharacterTable characterTable;
    [Tooltip("캐릭터 이름")] public string character_Name;
    [Tooltip("본인 오브젝트")] public GameObject character_Object;

    public enum FriendlyAndEnemy
    {
        Friendly,
        Enemy,
    }
    [Tooltip("아군 적군 캐릭터의 구분")] public FriendlyAndEnemy friendlyAndEnemy;

    [Header("캐릭터 전투 정보")]
    [Tooltip("캐릭터 체력")] public int character_Hp;
    [Tooltip("캐릭터 최대체력")] public int character_MaxHp;
    [Tooltip("캐릭터 현재보호막")] public int character_ShieldGauge = 0;
    [Tooltip("보유중인 페이지")] public PageTable[] character_PageDeck;
    [SerializeField, Tooltip("활성화된 페이지 번호, 몇번페이지까지 활성화가 되있는지")] private int character_PageActiveNumber = 5;
    [SerializeField, Range(0, 10), Tooltip("현재 잉크게이지")] public int character_InkGauge;
    [SerializeField, Range(0, 10), Tooltip("턴당 잉크게이지 회복량")] public int character_InkGaugeCharge;
    [SerializeField, Tooltip("추가 데미지")] public int character_AddDamage;
    [SerializeField, Tooltip("추가 명중률")] public int character_AddAccuracy;

    [System.Serializable]
    public class CharacterBuff
    {
        [SerializeField, Tooltip("버프 수치")] public int character_BuffValue = 0;
        [SerializeField, Tooltip("적용턴수")] public int character_BuffTurn = 0;
    }

    #region 버프
    [Header("버프")]
    [Tooltip("증폭 (주는 데미지 증가)")] public CharacterBuff character_Amplification;
    [Tooltip("조준 (명중률 증가")] public CharacterBuff character_Aiming;
    #endregion
    #region 전용버프
    [Header("마침표 (휴고 전용 버프")] public CharacterBuff character_HugoBuff;
    [Header("클라이맥스 (헤이즐 전용 버프")] public CharacterBuff character_HazelBuff;
    #endregion
    #region 디버프
    [Header("디버프")]
    [Tooltip("약화 (주는 데미지 감소")] public CharacterBuff character_Weakly;
    [Tooltip("실명 (명중률 감소)")] public CharacterBuff character_Blindness;
    [Tooltip("허약 (받는 데미지 증가)")] public CharacterBuff character_Wakness;
    #endregion
    #region 전용디버프
    [Header("전용 디버프")]
    [Tooltip("화약 (엘마 전용 디버프")] public CharacterBuff character_ElmaDeBuff;
    #endregion

    #region 캐릭터 생존여부를 체크하는 상태
    public enum LivingState
    {
        Living,
        Die,
    }

    public LivingState livingState;
    #endregion

    [Header("전투 진행 시 정보")]
    [Tooltip("사용할 페이지")] public PageTable activePage;
    [Tooltip("페이지를 사용할 대상")] public CharacterBase targetCharacter;

    [Header("타겟이 지정될 시 상대가 이동할 위치")]
    [Tooltip("타겟팅트랜스폼")] public Transform targetingTransform;

    #region 캐릭터 리소스
    [Header("캐릭터 리소스")]
    [SerializeField, Tooltip("캐릭터 리소스 오브젝트")] private GameObject character_ResourceObject;
    [SerializeField, Tooltip("캐릭터 리소스 스프라이트")] public SpriteRenderer character_ResourceSprite;
    [SerializeField, Tooltip("캐릭터 발사체")] private GameObject character_Projectile;
    [SerializeField, Tooltip("캐릭터 발사체 오브젝트")] private GameObject character_ProjectileObject;
    [SerializeField, Tooltip("애니메이션")] private Animator character_animator;
    #endregion
    #region 데미지 텍스트
    [Header("데미지 텍스트")]
    [SerializeField, Tooltip("데미지 UI")] private GameObject character_DamageUI;
    [SerializeField, Tooltip("데미지 UI 텍스트")] private Image[] character_DamageText;
    [SerializeField, Tooltip("힐 UI")] private GameObject character_HealUI;
    [SerializeField, Tooltip("힐 UI 텍스트")] private TMP_Text character_HealText;
    [SerializeField, Tooltip("보호막 UI")] private GameObject character_ShieldUI;
    [SerializeField, Tooltip("보호막 UI 텍스트")] private TMP_Text character_ShieldText;
    [SerializeField, Tooltip("보호막 UI")] private GameObject character_MissUI;
    [SerializeField, Tooltip("데미지폰트")] private Sprite[] damageFont;
    #endregion
    #region 카메라 위치
    [Header("카메라 위치")]
    [Tooltip("캐릭터 선택시 카메라 좌표")] public Transform character_SelectView;
    [Tooltip("캐릭터 이동시 카메라 좌표")] public Transform character_MoveView;
    [Tooltip("캐릭터 전투시 카메라 좌표")] public Transform character_BattleView;
    [Tooltip("캐릭터 투사체 카메라 좌표")] public Transform character_ProjectileView;
    #endregion
    #region 연동스크립트
    [Header("연동 스크립트")]
    [Tooltip("배틀시스템")] BattleSystem battleSystem;
    [Tooltip("카메라 매니저")] CameraManager cameraManager;
    [Tooltip("배틀UI")] BattleUI battleUI;
    #endregion

    private Transform originTransform;
    private float moveSpeed = 5f;

    private void Start()
    {
        character_Object = this.gameObject;

        if (originTransform == null)
        {
            GameObject go = new GameObject($"{name}_Origin");
            originTransform = go.transform;
            originTransform.position = transform.position;
        }

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

        // 카메라 매니저 찾기
        if (battleUI == null)
        {
            battleUI = FindFirstObjectByType<BattleUI>();
            if (battleUI == null)
            {
                Debug.LogError("battleUIr를 씬에서 찾을 수 없습니다.");
            }
        }

        //TableLink();
        battleUI.CharacterUiDataUpdate();
    }

    // ↓ 턴 감소 + 만료 시 값 0으로
    private static void DecreaseAndZeroIfExpired(ref int turn, ref int value)
    {
        if (turn > 0)
        {
            turn--;
            if (turn == 0) value = 0;   // 방금 만료되면 값도 0
        }
        else
        {
            // 방어코드: 음수 들어오면 0으로 클램프 + 값도 0 유지
            if (turn < 0) turn = 0;
            value = 0;
        }
    }

    public void BuffTurnDecrease()
    {
        // ▼ 값 필드명이 character_BuffValue 인 경우
        DecreaseAndZeroIfExpired(ref character_Amplification.character_BuffTurn, ref character_Amplification.character_BuffValue);
        DecreaseAndZeroIfExpired(ref character_Aiming.character_BuffTurn, ref character_Aiming.character_BuffValue);
        DecreaseAndZeroIfExpired(ref character_HugoBuff.character_BuffTurn, ref character_HugoBuff.character_BuffValue);
        DecreaseAndZeroIfExpired(ref character_HazelBuff.character_BuffTurn, ref character_HazelBuff.character_BuffValue);
        DecreaseAndZeroIfExpired(ref character_Weakly.character_BuffTurn, ref character_Weakly.character_BuffValue);
        DecreaseAndZeroIfExpired(ref character_Blindness.character_BuffTurn, ref character_Blindness.character_BuffValue);
        DecreaseAndZeroIfExpired(ref character_Wakness.character_BuffTurn, ref character_Wakness.character_BuffValue); // (Weakness 오타면 나중에 교정!)
        DecreaseAndZeroIfExpired(ref character_ElmaDeBuff.character_BuffTurn, ref character_ElmaDeBuff.character_BuffValue);
    }
    #region 최종 계산식
    //명중률 계산식
    int FinalAccuracy()
    {
        return activePage.page_Accuracy + ((activePage.page_Accuracy * character_Aiming.character_BuffValue) / 100);
    }
    //데미지 계산식
    int FinalDamage()
    {
        int damage = Random.Range(activePage.page_MinDamage, activePage.page_MaxDamage);
        int finalDamage = damage + ((damage * character_Amplification.character_BuffValue) / 100) - ((damage* character_Weakly.character_BuffValue)/100);
        return finalDamage;
    }

    #endregion
    #region 페이지타입을 확인하고 페이지 적용
    public void PageActive()
    {
        
        battleUI.CharacterUiDataUpdate();
        #region 페이지 타입 확인
        switch (activePage.pageActiveType)
        {
            #region Single_Attack 적군 한명에게 대상 공격
            case PageTable.PageActiveType.Single_Attack:
                //명중률 계산
                if (FinalAccuracy() > Random.Range(1, 101))
                {
                    //명중!!
                    Debug.Log("명중!!");
                    targetCharacter.Anima_Action("Hit", true);
                    // 상하로 0.6 유닛, 10Hz, 0.4초 흔들기
                    //cameraManager.ShakeVertical(cameraManager.allView, 0.6f, 10f, 0.4f);
                    if (friendlyAndEnemy == FriendlyAndEnemy.Friendly)
                    {
                        cameraManager.target_Camera = cameraManager.friendlyBattleActionTransfrom[activePage.page_AnimaTransformNumber];
                    }
                    else
                    {
                        cameraManager.target_Camera = cameraManager.enemyBattleActionTransfrom[activePage.page_AnimaTransformNumber];
                    }
                    // friendlyBattleActionTransfrom[2] 위치로 1.5초 동안 부드럽게 이동
                    //cameraManager.MoveCameraToFriendlyAction(0, 0.25f);
                    // 좌우 기본 세팅으로 흔들기
                    //cameraManager.ShakeHorizontal(cameraManager.target_Camera);
                    int damage = FinalDamage();
                    targetCharacter.character_Hp -= damage;
                    targetCharacter.DamageUiPrintOn(damage);
                    Debug.Log("데미지!!");
                    BuffEnchant();
                    Debug.Log("버프적용");
                    StartCoroutine(BuffActive_BattleFinish());
                    Debug.Log("배틀피니쉬 버프처리");
                    cameraManager.ShakeRandom(cameraManager.target_Camera, 0.9f, 14f, 0.4f);
                }
                else
                {
                    //빗나감!!
                    Debug.Log("빗나감!!");
                    targetCharacter.MissUiPrintOn();
                    // 상하로 0.6 유닛, 10Hz, 0.4초 흔들기
                    //cameraManager.ShakeVertical(cameraManager.allView, 0.6f, 10f, 0.4f);
                    // 좌우 기본 세팅으로 흔들기
                    if (friendlyAndEnemy == FriendlyAndEnemy.Friendly)
                    {
                        cameraManager.target_Camera = cameraManager.friendlyBattleActionTransfrom[activePage.page_AnimaTransformNumber];
                    }
                    else
                    {
                        cameraManager.target_Camera = cameraManager.enemyBattleActionTransfrom[activePage.page_AnimaTransformNumber];
                    }
                    cameraManager.ShakeHorizontal(cameraManager.target_Camera);
                }
                
                break;
            #endregion

            #region Aoe_Attack 적군에게 광역공격
            case PageTable.PageActiveType.Aoe_Attack:
                for (int i = 0; i < battleSystem.character_Enemy.Length; i++)
                {
                    if (activePage.page_Accuracy > Random.Range(1, 101))
                    {
                        if (battleSystem.character_Enemy[i].livingState == LivingState.Living)
                        {
                            Debug.Log(i + "번째 캐릭터에게 명중!");
                            int aoeDamage = Random.Range(activePage.page_MinDamage, activePage.page_MaxDamage);
                            battleSystem.character_Enemy[i].character_Hp -= aoeDamage;
                            battleSystem.character_Enemy[i].DamageUiPrintOn(aoeDamage);
                        }
                        else
                        {
                            Debug.Log(i + "번째 캐릭터가 사망한 상태라 명중불가");
                        }
                        
                    }
                    else
                    {
                        Debug.Log(i + "번째 캐릭터에게 빗나감!");
                        battleSystem.character_Enemy[i].MissUiPrintOn();
                    }
                }
                break;
            #endregion

            #region Single_Heal 아군 한명에게 대상 힐
            case PageTable.PageActiveType.Single_Heal:
                int healAmount = Random.Range(activePage.page_MinDamage, activePage.page_MaxDamage);
                targetCharacter.character_Hp += healAmount;

                // 최대 HP를 넘지 않도록 조정
                targetCharacter.character_Hp = Mathf.Min(targetCharacter.character_Hp, targetCharacter.character_MaxHp);

                targetCharacter.HealUiPrintOn(healAmount);

                Debug.Log($"힐!! {healAmount} 회복, 현재 HP: {targetCharacter.character_Hp}/{targetCharacter.character_MaxHp}");
                break;
            #endregion

            #region My_Heal 자기자신에게 힐
            case PageTable.PageActiveType.My_Heal:
                character_Hp += Random.Range(activePage.page_MinDamage, activePage.page_MaxDamage);
                break;
            #endregion

            #region Aoe_Heal 아군 전체에게 힐
            case PageTable.PageActiveType.Aoe_Heal:
                break;
            #endregion

            #region Single_Shield 아군 한명에게 보호막
            case PageTable.PageActiveType.Single_Shield:
                targetCharacter.character_ShieldGauge = 0;
                targetCharacter.character_ShieldGauge += Random.Range(activePage.page_MinDamage, activePage.page_MaxDamage);
                break;
            #endregion

            #region My_Shield 자기자신에게 보호막
            case PageTable.PageActiveType.My_Shield:
                character_ShieldGauge = 0;
                character_ShieldGauge += Random.Range(activePage.page_MinDamage, activePage.page_MaxDamage);
                break;
            #endregion

            #region Aoe_Shield 아군 전체에게 보호막
            case PageTable.PageActiveType.Aoe_Shield:
                break;
            #endregion
        }
        #endregion
        battleUI.CharacterUiDataUpdate();
    }
    #endregion
    #region 버프/디버프 부여
    public void BuffEnchant()
    {
        for (int i = 0; i < activePage.page_Buffs.Length; i++)
        {
            switch (activePage.page_Buffs[i].page_BuffUse)
            {
                #region 버프 적용 'My'
                case PageTable.BuffUse.My:
                    switch (activePage.page_Buffs[i].page_BuffType)
                    {
                        #region 공용버프
                        case PageTable.BuffType.Amplifucation:
                            character_Amplification.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            character_Amplification.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.Aiming:
                            character_Aiming.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            character_Aiming.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        #endregion
                        #region 전용 버프
                        case PageTable.BuffType.HugoBuff:
                            character_HugoBuff.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            character_HugoBuff.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.HazelBuff:
                            character_HazelBuff.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            character_HazelBuff.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        #endregion
                        #region 공용디버프
                        case PageTable.BuffType.Weakly:
                            character_Weakly.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            character_Weakly.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.Blindness:
                            character_Blindness.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            character_Blindness.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.Wakness:
                            character_Wakness.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            character_Wakness.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        #endregion
                        #region 전용디버프
                        case PageTable.BuffType.ElmaDeBuff:
                            character_ElmaDeBuff.character_BuffValue += activePage.page_Buffs[i].page_BuffValue;
                            character_ElmaDeBuff.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        #endregion
                        #region 특수 버프
                        #endregion
                        #region 특수 디버프
                            
                        #endregion
                    }
                    break;
                #endregion
                case PageTable.BuffUse.FriendlyAll:
                    break;
                #region 버프 적용 'Target'
                case PageTable.BuffUse.Target:
                    switch (activePage.page_Buffs[i].page_BuffType)
                    {
                        #region 공용버프
                        case PageTable.BuffType.Amplifucation:
                            targetCharacter.character_Amplification.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_Amplification.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.Aiming:
                            targetCharacter.character_Aiming.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_Aiming.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        #endregion
                        #region 전용 버프
                        case PageTable.BuffType.HugoBuff:
                            targetCharacter.character_HugoBuff.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_HugoBuff.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.HazelBuff:
                            targetCharacter.character_HazelBuff.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_HazelBuff.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        #endregion
                        #region 공용디버프
                        case PageTable.BuffType.Weakly:
                            targetCharacter.character_Weakly.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_Weakly.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.Blindness:
                            targetCharacter.character_Blindness.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_Blindness.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        case PageTable.BuffType.Wakness:
                            targetCharacter.character_Wakness.character_BuffValue = activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_Wakness.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                        #endregion
                        #region 전용디버프
                        case PageTable.BuffType.ElmaDeBuff:
                            targetCharacter.character_ElmaDeBuff.character_BuffValue += activePage.page_Buffs[i].page_BuffValue;
                            targetCharacter.character_ElmaDeBuff.character_BuffTurn = activePage.page_Buffs[i].page_BuffTurn;
                            break;
                            #endregion
                    }
                    break;
                #endregion
                case PageTable.BuffUse.EnemyAll:
                    break;

            }
        }
    }
    #endregion
    #region 버프/디버프 적용
    //스타트 페이즈
    public void BuffActive_StrtPhase()
    {
        
    }
    public IEnumerator BuffActive_EndPhase()
    {
        

        yield return new WaitForSeconds(1.0f);
        DamageUiPrintOff();

        yield return null;
    }
    public IEnumerator BuffActive_BattleFinish() 
    {
        


        for (int i = 0; i < activePage.page_Buffs.Length; i++)
        {
            //엘마 디버프가 3이상있을 경우 기본 30 + (버프수치-3)*15
            if (targetCharacter.character_ElmaDeBuff.character_BuffValue >= 3)
            {
                yield return new WaitForSeconds(1.0f);
                targetCharacter.DamageUiPrintOn(30 + ((character_ElmaDeBuff.character_BuffValue - 3) * 15));
                targetCharacter.character_Hp -= 30 + ((character_ElmaDeBuff.character_BuffValue - 3) * 15);
                targetCharacter.targetCharacter = this;

                yield return new WaitForSeconds(0.5f);
                targetCharacter.DamageUiPrintOff();

            }

            //어그로 버프처리
            if (activePage.page_Buffs[i].page_BuffType == PageTable.BuffType.Aggro)
            {
                targetCharacter.targetCharacter = this;
            }
        }

        yield return null;

    }
    #endregion
    #region 페이지에 따른 애니메이션 출력
    Transform myP1, tgtP1, myP2, tgtP2;
    public IEnumerator PageAnima()
    {
        //페이지 잉크게이지소모
        character_InkGauge -= activePage.page_InkGauge;
        battleUI.CharacterUiDataUpdate();

        switch (activePage.pageActiveType)
        {
            #region 단일 공격 시 연출
            case PageTable.PageActiveType.Single_Attack:
                
                switch (activePage.pageAnimaType)
                {
                    #region Anima1 연출
                    case PageTable.PageAnimaType.Anima1:
                        cameraManager.target_Camera = cameraManager.allView;
                        Debug.Log("all뷰변경");
                        //전투 진행시 연출을 위해 필터온
                        battleSystem.FilterOn();
                        //전투 애니메이션 진행
                        //cameraManager.target_Camera = character_BattleView;
                        //Debug.Log("배틀뷰변경");
                        // 목적지들 정리
                        
                        switch (friendlyAndEnemy)
                        {
                            case FriendlyAndEnemy.Friendly:
                                myP1 = cameraManager.friendlyBattleTransform1;
                                tgtP1 = cameraManager.enemyBattleTransform1;
                                myP2 = cameraManager.friendlyBattleTransform2;
                                tgtP2 = cameraManager.enemyBattleTransform2;
                                break;
                            case FriendlyAndEnemy.Enemy:
                                myP1 = cameraManager.enemyBattleTransform1;
                                tgtP1 = cameraManager.friendlyBattleTransform1;
                                myP2 = cameraManager.enemyBattleTransform2;
                                tgtP2 = cameraManager.friendlyBattleTransform2;
                                break;
                            default:
                                myP1 = myP2 = tgtP1 = tgtP2 = null;
                                break;
                        }
                        cameraManager.BlurFilterOnOff(0, true);
                        // === 1단계: 기존 위치 -> 지점1 "순간이동"(스냅)
                        if (myP1 != null) transform.SetPositionAndRotation(myP1.position, myP1.rotation);
                        if (tgtP1 != null) targetCharacter.transform.SetPositionAndRotation(tgtP1.position, tgtP1.rotation);

                        // 스케일 업
                        character_ResourceObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        targetCharacter.character_ResourceObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


                        // === 2단계: 지점1 -> 지점2 "부드럽게 이동"(동시에)
                        if (myP2 != null || tgtP2 != null)
                        {
                            yield return StartCoroutine(MovePairTo(
                                transform, myP2 != null ? myP2.position : transform.position,
                                targetCharacter.transform, tgtP2 != null ? tgtP2.position : targetCharacter.transform.position,
                                moveSpeed
                            ));
                        }

                        Anima_Action(activePage.page_AnimaName, true);
                        yield return new WaitForSeconds(0.1f);
                        Anima_Action(activePage.page_AnimaName, false);
                        yield return new WaitForSeconds(activePage.page_AnimaTime1);

                        PageActive();

                        yield return new WaitForSeconds(activePage.page_AnimaTime2);
                        targetCharacter.Anima_Action("Hit", false);

                        character_ResourceObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        targetCharacter.character_ResourceObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        transform.position = originTransform.position;
                        transform.rotation = originTransform.rotation;
                        targetCharacter.transform.position = targetCharacter.originTransform.position;
                        targetCharacter.transform.rotation = targetCharacter.originTransform.rotation;
                        battleSystem.AllLivingCheck();
                        targetCharacter.DamageUiPrintOff();
                        targetCharacter.MissUiPrintOff();
                        //필터 off
                        cameraManager.BlurFilterOnOff(0, false);
                        battleSystem.FilterOff();
                        break;
                    #endregion
                    

                        /*
                    #region 근거리연출
                    case PageTable.PageAnimaType.Melee:
                        
                        //전투 진행시 연출을 위해 필터온
                        battleSystem.FilterOn();
                        //캐릭터를 대상에게 이동                
                        StartCoroutine(MoveToTarget(targetCharacter.targetingTransform));                       
                        yield return new WaitForSeconds(0.5f);
                        //카메라 뷰를 배틀뷰로 변경
                        cameraManager.target_Camera = character_BattleView;
                        Debug.Log("배틀뷰변경");
                        //페이지에 해당하는 애니메이션 실행
                        Anima_Action(activePage.page_AnimaName, true);
                        yield return new WaitForSeconds(0.3f);
                        Anima_Action(activePage.page_AnimaName, false);
                        //연출 중에 피격모션과 데미지실행
                        yield return new WaitForSeconds(activePage.page_AnimaTime1);
                        //명중률계산!
                        if (activePage.page_Accuracy > Random.Range(1, 101))
                        {
                            //명중!!
                            Debug.Log("명중!!");
                            targetCharacter.Anima_Action("Hit", true);
                            //페이지적용
                            PageActive();
                            yield return new WaitForSeconds(activePage.page_AnimaTime2);
                            targetCharacter.Anima_Action("Hit", false);
                        }
                        else
                        {
                            //빗나감
                            yield return new WaitForSeconds(activePage.page_AnimaTime2);
                            Debug.Log("빗나감!!");
                        }
                        //데미지ui off
                        targetCharacter.DamageUiPrintOff();
                        targetCharacter.MissUiPrintOff();
                        //원래 위치로 돌아감
                        StartCoroutine(MoveToBack());
                        //필터 off
                        battleSystem.FilterOff();
                        break;
                    #endregion
                    #region 원거리 연출
                    case PageTable.PageAnimaType.Ranged:
                        cameraManager.target_Camera = cameraManager.allView;
                        Debug.Log("all뷰변경");
                        //전투 진행시 연출을 위해 필터온
                        battleSystem.FilterOn();
                        //전투 애니메이션 진행
                        //cameraManager.target_Camera = character_BattleView;
                        //Debug.Log("배틀뷰변경");
                        // 목적지들 정리
                        
                        switch (friendlyAndEnemy)
                        {
                            case FriendlyAndEnemy.Friendly:
                                myP1 = cameraManager.friendlyBattleTransform1;
                                tgtP1 = cameraManager.enemyBattleTransform1;
                                myP2 = cameraManager.friendlyBattleTransform2;
                                tgtP2 = cameraManager.enemyBattleTransform2;
                                break;
                            case FriendlyAndEnemy.Enemy:
                                myP1 = cameraManager.enemyBattleTransform1;
                                tgtP1 = cameraManager.friendlyBattleTransform1;
                                myP2 = cameraManager.enemyBattleTransform2;
                                tgtP2 = cameraManager.friendlyBattleTransform2;
                                break;
                            default:
                                myP1 = myP2 = tgtP1 = tgtP2 = null;
                                break;
                        }
                        cameraManager.BlurFilterOnOff(0, true);
                        // === 1단계: 기존 위치 -> 지점1 "순간이동"(스냅)
                        if (myP1 != null) transform.SetPositionAndRotation(myP1.position, myP1.rotation);
                        if (tgtP1 != null) targetCharacter.transform.SetPositionAndRotation(tgtP1.position, tgtP1.rotation);

                        // 스케일 업
                        character_ResourceObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        targetCharacter.character_ResourceObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


                        // === 2단계: 지점1 -> 지점2 "부드럽게 이동"(동시에)
                        if (myP2 != null || tgtP2 != null)
                        {
                            yield return StartCoroutine(MovePairTo(
                                transform, myP2 != null ? myP2.position : transform.position,
                                targetCharacter.transform, tgtP2 != null ? tgtP2.position : targetCharacter.transform.position,
                                moveSpeed
                            ));
                        }

                        Anima_Action(activePage.page_AnimaName, true);
                        yield return new WaitForSeconds(0.1f);
                        Anima_Action(activePage.page_AnimaName, false);
                        yield return new WaitForSeconds(activePage.page_AnimaTime1);
                        
                        //총알이 날라가는 모션진행
                        //카메라 뷰를 투사체뷰로 변경
                        //cameraManager.target_Camera= character_ProjectileView;
                        //StartCoroutine(ProjectileMoveToTarget(targetCharacter.targetingTransform));
                        //명중률계산!
                        if (activePage.page_Accuracy > Random.Range(1, 101))
                        {
                            //명중!!
                            Debug.Log("명중!!");
                            targetCharacter.Anima_Action("Hit", true);
                            PageActive();
                            yield return new WaitForSeconds(activePage.page_AnimaTime2);
                            targetCharacter.Anima_Action("Hit", false);
                        }
                        else 
                        {
                            //빗나감!!
                            Debug.Log("빗나감!!");

                            yield return new WaitForSeconds(activePage.page_AnimaTime2);
                        }
                        character_ResourceObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        targetCharacter.character_ResourceObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        transform.position = originTransform.position;
                        transform.rotation = originTransform.rotation;
                        targetCharacter.transform.position = targetCharacter.originTransform.position;
                        targetCharacter.transform.rotation = targetCharacter.originTransform.rotation;
                        battleSystem.AllLivingCheck();
                        //데미지UI Off
                        targetCharacter.DamageUiPrintOff();
                        targetCharacter.MissUiPrintOff();
                        //필터 off
                        cameraManager.BlurFilterOnOff(0, false);
                        battleSystem.FilterOff();
                        break;
                    #endregion
                        */
                }
                break;
            #endregion

            #region 광역 공격 시 연출
            case PageTable.PageActiveType.Aoe_Attack:
                switch (activePage.pageAnimaType)
                {
                    case PageTable.PageAnimaType.Anima1:
                        PageActive();
                        yield return new WaitForSeconds(1.0f);
                        for (int i = 0; i < battleSystem.character_Enemy.Length; i++)
                        {
                            battleSystem.character_Enemy[i].DamageUiPrintOff();
                        }
                        break;
                    case PageTable.PageAnimaType.Anima2:
                        PageActive();
                        yield return new WaitForSeconds(1.0f);
                        for (int i = 0; i < battleSystem.character_Enemy.Length; i++)
                        {
                            battleSystem.character_Enemy[i].DamageUiPrintOff();
                        }
                        break;
                }
                break;
            #endregion
            #region 단일 회복 연출
            case PageTable.PageActiveType.Single_Heal:
                switch (activePage.pageAnimaType)
                {
                    case PageTable.PageAnimaType.Anima1:
                        break;
                    case PageTable.PageAnimaType.Anima2:
                        //명중률계산!
                        if (activePage.page_Accuracy > Random.Range(1, 101))
                        {
                            Debug.Log("힐 명중!");
                            PageActive();
                        }
                        else
                        {
                            Debug.Log("힐 빗나감!");
                        }
                        yield return new WaitForSeconds(1.0f);
                        targetCharacter.HealUiPrintOff();
                        break;
                }
            break;
            #endregion

        }
        battleSystem.ColorASet(1.0f);
        //아군 페이지액티브 턴이었으면 다시 캐릭터선택페이즈로 복귀
        if (battleSystem.battleTurn == BattleSystem.BattleTurn.Friendly_PageActivePhase) {
            battleSystem.battleTurn = BattleSystem.BattleTurn.Friendly_CharacterSelectPhase;
        }
       
        battleSystem.AllLivingCheck();
        yield return null;
    }
    #endregion

    #region 이동관련
    
    //타겟에게 이동
    IEnumerator MoveToTarget(Transform target)
    {
        //cameraManager.target_Camera = character_MoveView;
        Anima_Action("Run", true);
        if (target == null) yield break;

        Vector3 targetPos = target.position;

        // X/Y축 임계치 1f, Z축 임계치 0.1f
        while (Mathf.Abs(transform.position.x - targetPos.x) > 0.01f
            || Mathf.Abs(transform.position.y - targetPos.y) > 0.01f
            || Mathf.Abs(transform.position.z - targetPos.z) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = originTransform.position;
        transform.rotation = originTransform.rotation;
        targetCharacter.transform.position = targetCharacter.originTransform.position;
        targetCharacter.transform.rotation = targetCharacter.originTransform.rotation;
        //cameraManager.target_Camera = null;
        Anima_Action("Run", false);
    }
    //원래 위치로 복귀
    IEnumerator MoveToBack()
    {
        if (originTransform == null) yield break;
        Anima_Action("Run", true);
        while (Vector3.Distance(transform.position, originTransform.position) >= 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                originTransform.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = originTransform.position;
        transform.rotation = originTransform.rotation;
        targetCharacter.transform.position = targetCharacter.originTransform.position;
        targetCharacter.transform.rotation = targetCharacter.originTransform.rotation;
        Anima_Action("Run", false);
        
    }

    IEnumerator ProjectileMoveToTarget(Transform target)
    {
        // 이동 전 위치 저장
        Vector3 originalPos = character_Projectile.transform.position;
        character_ProjectileObject.SetActive(true);

        if (target == null) yield break;

        Vector3 targetPos = target.position;

        // X/Y축 임계치 1f, Z축 임계치 0.1f
        while (Mathf.Abs(character_Projectile.transform.position.x - targetPos.x) > 0.01f
            || Mathf.Abs(character_Projectile.transform.position.y - targetPos.y) > 0.01f
            || Mathf.Abs(character_Projectile.transform.position.z - targetPos.z) > 0.01f)
        {
            character_Projectile.transform.position = Vector3.MoveTowards(
                character_Projectile.transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        character_ProjectileObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);


        character_Projectile.transform.position = target.position;

        // 원래 위치로 즉시 순간이동
        character_Projectile.transform.position = originalPos;
    }

    /// <summary>
    /// 두 개의 트랜스폼을 동시에 target으로 MoveTowards로 이동시킨다.
    /// 둘 다 목표에 도달할 때까지 대기.
    /// </summary>
    IEnumerator MovePairTo(Transform a, Vector3 aTarget, Transform b, Vector3 bTarget, float speed)
    {
        if (a == null && b == null) yield break;

        // 도착 판정 오차
        const float epsilon = 0.01f;

        while (true)
        {
            bool aDone = true, bDone = true;

            if (a != null)
            {
                if ((a.position - aTarget).sqrMagnitude > epsilon * epsilon)
                {
                    a.position = Vector3.MoveTowards(a.position, aTarget, speed * Time.deltaTime);
                    aDone = false;
                }
            }

            if (b != null)
            {
                if ((b.position - bTarget).sqrMagnitude > epsilon * epsilon)
                {
                    b.position = Vector3.MoveTowards(b.position, bTarget, speed * Time.deltaTime);
                    bDone = false;
                }
            }

            if (aDone && bDone) break;
            yield return null;
        }

        // 최종 보정
        if (a != null) a.position = aTarget;
        if (b != null) b.position = bTarget;
    }

    #endregion

    #region 데미지UI
    private void DamageUiPrintOn(int damage)
    {
        character_DamageUI.SetActive(true);

        // 1) 모두 숨김
        for (int i = 0; i < character_DamageText.Length; i++)
        {
            character_DamageText[i].enabled = false;
            character_DamageText[i].sprite = null;
        }

        // 2) 음수 들어오면 절대값으로, 9999 초과면 마지막 4자리만 사용
        int v = Mathf.Abs(damage);
        string s = v.ToString();
        if (s.Length > 4) s = s.Substring(s.Length - 4);

        // 3) 자리수에 따른 시작 슬롯 결정
        int len = s.Length;
        int startSlot =
            (len == 1) ? 2 :   // 3번 이미지 (index 2)
            (len == 2) ? 1 :   // 2,3번 (1~2)
            (len == 3) ? 1 :   // 2,3,4번 (1~3)
            0;                 // 1~4번 (0~3)

        // 4) 각 자릿수를 해당 슬롯에 채우기
        for (int i = 0; i < len; i++)
        {
            int digit = s[i] - '0';
            int slot = startSlot + i;     // 채울 이미지 슬롯

            if (slot >= 0 && slot < character_DamageText.Length &&
                digit >= 0 && digit <= 9)
            {
                character_DamageText[slot].sprite = damageFont[digit];
                character_DamageText[slot].enabled = true;
            }
        }

    }
    private void DamageUiPrintOff()
    {
        character_DamageUI.SetActive(false);

    }

    private void HealUiPrintOn(int damage)
    {
        character_HealUI.SetActive(true);
        character_HealText.text = damage.ToString();
    }
    private void HealUiPrintOff()
    {
        character_HealUI.SetActive(false);
    }

    private void ShieldUiPrintOn(int damage)
    {
        character_ShieldUI.SetActive(true);
        character_ShieldText.text = damage.ToString();
    }
    private void ShieldUiPrintOff()
    {
        character_ShieldUI.SetActive(false);
    }

    private void MissUiPrintOn()
    {
        character_MissUI.SetActive(true);
    }
    private void MissUiPrintOff()
    {
        character_MissUI.SetActive(false);
    }

    #endregion

    #region 회전값트레킹 
    /// <summary>
    /// 해당 캐릭터의 리소스가 타켓의 회전값에 따라 회전하는 함수입니다.
    /// </summary>
    /// <param name="Target", 회전값을 받을 타겟></param>
    public void RotationTracking(Transform Target)
    {
        // 현재 스프라이트의 회전
        Vector3 currentEuler = character_ResourceObject.transform.rotation.eulerAngles;

        // 카메라의 회전 값 가져오기
        Vector3 targetEuler = Target.rotation.eulerAngles;

        // Y축은 유지하고, X/Z만 카메라를 따르도록 설정
        Vector3 newEuler = new Vector3(-targetEuler.x, currentEuler.y, targetEuler.z);

        // 부드럽게 회전 적용
        Quaternion quaternion = Quaternion.Lerp(character_ResourceObject.transform.rotation, Quaternion.Euler(newEuler), 20 * Time.deltaTime);
        character_ResourceObject.transform.rotation = quaternion;

        // 🔽 회전 각도를 -180 ~ 180 범위로 변환
        float rotationX = character_ResourceObject.transform.rotation.eulerAngles.x;
        if (rotationX > 180f) rotationX -= 360f;

        // 🔁 현재 위치 받아오기
        Vector3 currentPos = character_ResourceObject.transform.position;

        // ✅ 목표 y값 결정
        float targetY = (rotationX < -70f) ? 0.6f : 1f;

        // ✅ y값만 부드럽게 Lerp로 이동
        character_ResourceObject.transform.position = currentPos;
    }
    #endregion
    #region 테이블 링크
    /// <summary>
    /// 캐릭터 생성시, 캐릭터가 보유한 테이블의 정보를 갱신합니다.
    /// (1회실행)
    /// </summary>
    public void TableLink()
    {
        //이름 링크
        character_Name = characterTable.character_Name;
        //HP 링크
        character_MaxHp = characterTable.character_Hp;
        character_Hp = character_MaxHp;
        //잉크게이지 회복량 링크
        character_InkGaugeCharge = characterTable.character_InkGaugeCharge;
        //페이지 링크
        for(int i=0; i<character_PageActiveNumber; i++) 
        {
            character_PageDeck[i] = characterTable.character_PageTable[i];
        }
        //애니메이션 링크
        character_animator.runtimeAnimatorController = characterTable.animator;
    }
    #endregion
    #region 애니메이션 출력
    /// <summary>
    /// 캐릭터 애니메이션을 출력합니다.
    /// </summary>
    /// <param name="animaName", 출력할 애니메이션 이름></param>
    /// <param name="check", true, false 여부 선택></param>
    public void Anima_Action(string animaName, bool check)
    {
        character_animator.SetBool(animaName, check);
    }
    #endregion
    #region 스프라이트 투명도 조절
    /// <summary>
    /// 스프라이트의 투명도를 설정합니다. (0 = 완전 투명, 1 = 불투명)
    /// </summary>
    /// <param name="alpha">0.0f ~ 1.0f 사이 값</param>
    public void SetSpriteAlpha(float alpha)
    {
        if (character_ResourceSprite == null)
        {
            Debug.LogWarning("character_ResourceSprite가 할당되지 않았습니다.");
            return;
        }

        // 기존 컬러 가져오기
        Color c = character_ResourceSprite.color;
        // Alpha만 바꿔서 다시 할당
        c.a = Mathf.Clamp01(alpha);
        character_ResourceSprite.color = c;
    }
    #endregion

    #region HP에 따라 생존여부 변경
    public void LivingStateCheck()
    {
        if (character_Hp <= 0 && livingState == LivingState.Living) 
        {
            livingState = LivingState.Die;
            Anima_Action("Die", true);
        }
    }
    #endregion
}
