using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<BaseItem> Item_Info; //리스트로 아이템들 관리

    public void SetItem() //아이템 데이터를 가지고오는 방법
    {
        Item_Info = new List<BaseItem>();
        //Resources 폴더 안에 있는 Item 폴더 안의
        //모든 BaseItem 스크립트를 가지고 있는 데이터를 가져옴
        BaseItem[] loadedItems = Resources.LoadAll<BaseItem>("ItemData");

        //List에 추가
        Item_Info.AddRange(loadedItems);
    }

    public void Start()
    {
        SetItem();
    }
}
