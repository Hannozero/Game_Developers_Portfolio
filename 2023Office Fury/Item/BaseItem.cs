using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TYPE //아이템 타입
{
    TYPE_ACTIVE = 1,
    TYPE_PASSIVE
};

public enum RATING //아이템 레이팅
{
    RATING_BRONZE = 1,
    RATING_SILVER,
    RATING_GOLD,
    RATING_ACTIVE
};

/*BaseItem은 모든 아이템이 가지고 있는 공통된 정보입니다.
BaseItem을 부모로 삼아 추가되는 정보는 따로 저장해 두었습니다.
모든 아이템들은 ScriptableObject방식으로 만들었으며
각 아이템마다 값을 가지고 있기에 유지보수 하기 편할겁니다.*/
public class BaseItem : ScriptableObject
{
    public TYPE ItemType;
    public RATING ItemRating;
    public int ItemCode;
    public string ItemName;
    public string ItemExplan;
    public int ItemPrice;
}


