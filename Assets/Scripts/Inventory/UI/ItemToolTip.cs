using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 创建人：杜
 * 功能说明：
 * 创建时间：
 */

public class ItemToolTip : MonoBehaviour
{
    public Text itemNameText;
    public Text itemInfoText;

    private RectTransform rectTrans;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UpdatePosition();
        //强制刷新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void SetupTooltip(ItemData_SO item)
    {
        itemNameText.text = item.itemName;
        itemInfoText.text = item.description;
    }

    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;

        //用4个点数组表示rectTrans的四个顶点
        Vector3[] corners = new Vector3[4];
        rectTrans.GetWorldCorners(corners);
        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        if (mousePos.y < height&& mousePos.x > width)
            rectTrans.position = mousePos + Vector3.up * height * 0.51f + Vector3.left * width * 0.51f;
        else if (mousePos.y > height && mousePos.x < width)
            rectTrans.position = mousePos + Vector3.down * height * 0.51f + Vector3.right * width * 0.51f;
        else if(mousePos.y < height && mousePos.x < width)
            rectTrans.position = mousePos + Vector3.up * height * 0.51f + Vector3.right * width * 0.51f;
        else
            rectTrans.position = mousePos + Vector3.down * height * 0.51f + Vector3.left * width * 0.51f;
    }
}
