using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private const float nodeWidth = 160f;

    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    private const float connectingLineWidth = 3f;

    private const float connectingLineArrowSize = 6f;
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    public GUIStyle RoomNodeStyle { get => roomNodeStyle; set => roomNodeStyle = value; }
    public GUIStyle RoomNodeSelectedStyle { get => roomNodeSelectedStyle; set => roomNodeSelectedStyle = value; }
    public static RoomNodeGraphSO CurrentRoomNodeGraph { get => currentRoomNodeGraph; set => currentRoomNodeGraph = value; }
    public Vector2 GraphOffset { get => graphOffset; set => graphOffset = value; }
    public Vector2 GraphDrag { get => graphDrag; set => graphDrag = value; }
    public RoomNodeSO CurrentRoomNode { get => currentRoomNode; set => currentRoomNode = value; }
    public RoomNodeTypeListSO RoomNodeTypeList { get => roomNodeTypeList; set => roomNodeTypeList = value; }

    public static float NodeWidth => nodeWidth;
    public static float NodeHeight => nodeHeight;
    public static int NodePadding => nodePadding;
    public static int NodeBorder => nodeBorder;

    public static float ConnectingLineWidth => connectingLineWidth;

    public static float ConnectingLineArrowSize => connectingLineArrowSize;

    public static float GridLarge => gridLarge;

    public static float GridSmall => gridSmall;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;
        RoomNodeStyle = new GUIStyle();
        RoomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        RoomNodeStyle.normal.textColor = Color.yellow;
        RoomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
        RoomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);
        RoomNodeSelectedStyle = new GUIStyle();
        RoomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        RoomNodeSelectedStyle.normal.textColor = Color.yellow;
        RoomNodeSelectedStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
        RoomNodeSelectedStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

        // Load Room node types
        RoomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    [OnOpenAsset(0)]  
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            OpenWindow();

            CurrentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    private void OnGUI()
    {
        if (CurrentRoomNodeGraph != null)
        {
            DrawBackgroundGrid(GridSmall, 0.2f, Color.black);
            DrawBackgroundGrid(GridLarge, 0.3f, Color.gray);
            DrawDraggedLine();
            ProcessEvents(Event.current);
            DrawRoomConnections();
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        GraphOffset += GraphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(GraphOffset.x % gridSize, GraphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        Handles.color = Color.white;

    }


    private void DrawDraggedLine()
    {
        if (CurrentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(CurrentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, CurrentRoomNodeGraph.linePosition, CurrentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, CurrentRoomNodeGraph.linePosition, Color.white, null, ConnectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        GraphDrag = Vector2.zero;
        if (CurrentRoomNode == null || CurrentRoomNode.isLeftClickDragging == false)
        {
            CurrentRoomNode = IsMouseOverRoomNode(currentEvent);
        }
        if (CurrentRoomNode == null || CurrentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            CurrentRoomNode.ProcessEvents(currentEvent);
        }
    }

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = CurrentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (CurrentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return CurrentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);

                break;

            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject)
    {
        if (CurrentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), RoomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, RoomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        CurrentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialise(new Rect(mousePosition, new Vector2(NodeWidth, NodeHeight)), CurrentRoomNodeGraph, roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, CurrentRoomNodeGraph);
        AssetDatabase.SaveAssets();
        CurrentRoomNodeGraph.OnValidate();
    }

    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = CurrentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = CurrentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        while (roomNodeDeletionQueue.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            CurrentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            CurrentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            DestroyImmediate(roomNodeToDelete, true);
            AssetDatabase.SaveAssets();

        }
    }

    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childRoomNode = CurrentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        ClearAllSelectedRoomNodes();
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }

    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && CurrentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            if (roomNode != null)
            {
                if (CurrentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeIDToRoomNode(CurrentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            ClearLineDrag();
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (CurrentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        GraphDrag = dragDelta;

        for (int i = 0; i < CurrentRoomNodeGraph.roomNodeList.Count; i++)
        {
            CurrentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }
        GUI.changed = true;
    }


    public void DragConnectingLine(Vector2 delta)
    {
        CurrentRoomNodeGraph.linePosition += delta;
    }

    private void ClearLineDrag()
    {
        CurrentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        CurrentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void DrawRoomConnections()
    {
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    if (CurrentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, CurrentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;
        Vector2 midPosition = (endPosition + startPosition) / 2f;
        Vector2 direction = endPosition - startPosition;
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;
        Vector2 arrowHeadPoint = midPosition + direction.normalized * ConnectingLineArrowSize;
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, ConnectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, ConnectingLineWidth);
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, ConnectingLineWidth);

        GUI.changed = true;
    }

    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(RoomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(RoomNodeStyle);
            }
        }
        GUI.changed = true;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            CurrentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}