using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.IO;

/*
 * 创建人：杜
 * 功能说明：对话编辑器
 * 创建时间：
 */

// 所影响的数据类型，必写
[CustomEditor(typeof(DialogueData_SO))]
public class DialogueCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制一个按钮，点击后在 Dialogue Editor 窗口中打开
        if (GUILayout.Button("Open in Editor"))
        {
            DialogueEditor.InitWindow((DialogueData_SO)target);// target 可以指定当前选中的物体
        }

        // 绘制基本内容，写在上方或下方会根据代码顺序显示
        base.OnInspectorGUI();
    }
}

public class DialogueEditor : EditorWindow
{
    DialogueData_SO currentData;

    // 保存每一条对话的信息
    // ReorderableList 可调顺序的列表
    ReorderableList piecesList = null;

    // 滚动条位置
    Vector2 scrollPos = Vector2.zero;

    // 创建一个保存对话条目与选项对应的字典
    Dictionary<string, ReorderableList> optionListDict = new Dictionary<string, ReorderableList>();

    // 创建新增文件名的索引
    int dataAssetindex;

    // 创建选项
    [MenuItem("扩展/Dialogue Editor")]
    // 要静态的
    public static void Init()
    {
        // 创建新窗口
        DialogueEditor dialogueEditorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        // 根据场景变换重绘窗口
        dialogueEditorWindow.autoRepaintOnSceneChange = true;

    }

    // 创建窗口
    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor dialogueEditorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        dialogueEditorWindow.currentData = data;
    }

    [OnOpenAsset(1)]// Callbacks 的序号
    public static bool OpenAsset(int instanceID, int line)// 除了函数名，其他都是规定写法
    {
        DialogueData_SO data = EditorUtility.InstanceIDToObject(instanceID) as DialogueData_SO;

        if (data != null)
        {
            DialogueEditor.InitWindow(data);
            return true;
        }

        return false;
    }

    // 当切换选择时执行
    private void OnSelectionChange()
    {
        var newData = Selection.activeObject as DialogueData_SO;
        if (newData != null)
        {
            currentData = newData;

            // 重新绘制窗口
            SetupReorderableList();
        }
        else
        {
            currentData = null;
            piecesList = null;
        }

        // 强制执行一次 OnGUI()
        Repaint();
    }

    // 面板绘制，相当于 Update()
    private void OnGUI()
    {
        if (currentData != null)
        {
            EditorGUILayout.TextField(currentData.name, EditorStyles.boldLabel);// TextField 可点选高亮，LabelField 不行
            GUILayout.Space(10);

            // 设置滚动条开始
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (piecesList == null)
            {
                SetupReorderableList();
            }

            // 具体绘制
            // 显示一个对话条目的 List，与 SO 脚本对应，对其操作会反映到 SO 文件上
            piecesList.DoLayoutList();

            // 设置滚动条开始
            GUILayout.EndScrollView();
        }
        else
        {
            // 绘制按钮
            if (GUILayout.Button("Create New Dialogue"))
            {
                string dataPath = "Assets/Game Data/Dialogue Data/";
                if (Directory.Exists(dataPath) == false)
                    Directory.CreateDirectory(dataPath);

                // 创建实例化数据
                DialogueData_SO newData = ScriptableObject.CreateInstance<DialogueData_SO>();
                if (Directory.Exists(dataPath + "New Dialogue") == false)
                {
                    dataAssetindex = 0;
                    // 创建资源文件
                    AssetDatabase.CreateAsset(newData, dataPath + "New Dialogue.asset");
                }
                else
                {
                    dataAssetindex++;
                    AssetDatabase.CreateAsset(newData, dataPath + "New Dialogue" + "(" + dataAssetindex + ")" + ".asset");
                }

                currentData = newData;
            }

            GUILayout.Label("No Data Seleted!", EditorStyles.boldLabel);
        }
    }

    // 窗口关闭时
    private void OnDisable()
    {
        // 清空字典
        optionListDict.Clear();
    }

    private void SetupReorderableList()
    {
        // 创建对话条目的 ReorderableList
        piecesList = new ReorderableList(currentData.dialoguePieces, typeof(DialoguePiece), true, true, true, true);

        // 构件其中的选项
        // 添加绘制标题的 Callback
        piecesList.drawHeaderCallback += OnDrawPieceHeader;// 也可以使用 =
        // 添加绘制要素的 Callback
        piecesList.drawElementCallback += OnDrawPiecesListElement;
        // 添加绘制要素高度的 Callback
        piecesList.elementHeightCallback += OnHeightChanged;
    }

    private float OnHeightChanged(int index)
    {
        return GetPieceHeight(currentData.dialoguePieces[index]);
    }

    private float GetPieceHeight(DialoguePiece piece)
    {
        var height = EditorGUIUtility.singleLineHeight;

        var isExpand = piece.canExpand;

        if (isExpand == false)
            return height;

        height += EditorGUIUtility.singleLineHeight * 9;

        // 按 Option 增加高度
        var option = piece.options;
        if (option.Count > 0)
        {
            height += EditorGUIUtility.singleLineHeight * option.Count;
        }

        return height;
    }

    private void OnDrawPiecesListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        // 使当前操作的数据文件可以 保存、撤销操作
        EditorUtility.SetDirty(currentData);

        GUIStyle textStyle = new GUIStyle("TextField");// 创建时不给参数会将这个 GUIStyle 应用到所有文本类型的编辑字段上
        // 可以写上指定的应用位置，例如 "TextField"

        if (index < currentData.dialoguePieces.Count)
        {
            var currentPiece = currentData.dialoguePieces[index];

            var tempRect = rect;

            // 折叠箭头，显示ID
            tempRect.height = EditorGUIUtility.singleLineHeight;// singleLineHeight 默认单行高度
            currentPiece.canExpand = EditorGUI.Foldout(tempRect, currentPiece.canExpand, currentPiece.ID);

            // 如果没有展开，则直接 return
            if (currentPiece.canExpand == false)
                return;

            // 绘制 ID 标题
            tempRect.y += EditorGUIUtility.singleLineHeight;
            tempRect.width = 30;
            EditorGUI.LabelField(tempRect, "ID");

            // 绘制 ID 的 TextField
            tempRect.x += tempRect.width;
            tempRect.width = 100;
            currentPiece.ID = EditorGUI.TextField(tempRect, currentPiece.ID);

            // 绘制 Quest 标题
            tempRect.x += tempRect.width + 10;
            EditorGUI.LabelField(tempRect, "Quest");

            // 绘制获取 QuestData_SO 的按钮
            tempRect.x += tempRect.width;
            currentPiece.quest = (QuestData_SO)EditorGUI.ObjectField(tempRect, currentPiece.quest, typeof(QuestData_SO), false);

            // 绘制获取 Sprite 的按钮
            tempRect.y += EditorGUIUtility.singleLineHeight + 10;
            tempRect.x = rect.x;
            tempRect.height = 60;
            tempRect.width = tempRect.height;
            currentPiece.image = (Sprite)EditorGUI.ObjectField(tempRect, currentPiece.image, typeof(Sprite), false);

            // 绘制对话内容文本框
            tempRect.x += tempRect.width + 5;
            tempRect.width = rect.width - tempRect.x;
            textStyle.wordWrap = true;
            currentPiece.text = EditorGUI.TextField(tempRect, currentPiece.text, textStyle);

            // 绘制选项
            tempRect.y += tempRect.height + 5;
            tempRect.x = rect.x;
            tempRect.width = rect.width;
            string optionListKey = currentPiece.ID + currentPiece.text;// 加上 currentPiece.ID 避免重复
            if (optionListKey != string.Empty)
            {
                if (!optionListDict.ContainsKey(optionListKey))
                {
                    // 创建 ReorderableList
                    var optionList = new ReorderableList(currentPiece.options, typeof(DialogueOption), true, true, true, true);

                    optionList.drawHeaderCallback = OnDrawOptionHeader;

                    // 绘制内容的回调
                    optionList.drawElementCallback = (optionRect, optionIndex, optionActive, optionForcused) =>
                    {
                        OnDrawOptionElement(currentPiece, optionRect, optionIndex, optionActive, optionForcused);
                    };

                    // 匹配到字典中
                    optionListDict[optionListKey] = optionList;
                }

                optionListDict[optionListKey].DoList(tempRect);// DoLayoutList() 画在当前列表的面板中，DoList() 可以画在其他列表中
            }
        }
    }

    private void OnDrawOptionHeader(Rect rect)
    {
        // 绘制 选项文字 项目名
        rect.x += 50;
        GUI.Label(rect, "Option Text");

        // 绘制 目标ID 项目名
        rect.x += rect.width * 0.5f - 5;
        GUI.Label(rect, "Target ID");

        // 绘制 接受任务 项目名
        rect.x += rect.width * 0.3f - 15;
        GUI.Label(rect, "Quest");
    }

    private void OnDrawOptionElement(DialoguePiece currentPiece, Rect optionRect, int optionIndex, bool optionActive, bool optionForcused)
    {
        var currentOption = currentPiece.options[optionIndex];
        var tempRect = optionRect;

        // 绘制 Option 标题
        tempRect.width = 15;
        EditorGUI.LabelField(tempRect, optionIndex.ToString());

        // 绘制选项文字框
        tempRect.x += tempRect.width + 5;
        tempRect.width = optionRect.width * 0.5f;
        currentOption.text = EditorGUI.TextField(tempRect, currentOption.text);

        // 绘制 targetID
        tempRect.x += tempRect.width + 5;
        tempRect.width = optionRect.width * 0.3f;
        currentOption.targetID = EditorGUI.TextField(tempRect, currentOption.targetID);

        // 绘制接受任务的勾选框
        tempRect.x += tempRect.width + 15;
        tempRect.width = optionRect.width + 0.2f;
        currentOption.takeQuest = EditorGUI.Toggle(tempRect, currentOption.takeQuest);

        // 
        if (currentOption.takeQuest)
        {

        }
    }

    private void OnDrawPieceHeader(Rect rect)
    {
        GUI.Label(rect, "Dialogue Pieces");
    }
}
