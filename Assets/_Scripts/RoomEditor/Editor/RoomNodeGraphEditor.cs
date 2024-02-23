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

    // Node layout values
    private const float nodeWidth = 160f;

    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Connecting line values
    private const float connectingLineWidth = 3f;

    private const float connectingLineArrowSize = 6f;

    // Grid Spacing
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
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        // Define node layout style
        RoomNodeStyle = new GUIStyle();
        RoomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        RoomNodeStyle.normal.textColor = Color.yellow;
        RoomNodeStyle.padding = new RectOffset(NodePadding, NodePadding, NodePadding, NodePadding);
        RoomNodeStyle.border = new RectOffset(NodeBorder, NodeBorder, NodeBorder, NodeBorder);

        // Define selected node style
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
        // Unsubscribe from the inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    [OnOpenAsset(0)]  // Need the namespace UnityEditor.Callbacks
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
        // If a scriptable object of type RoomNodeGraphSO has been selected then process
        if (CurrentRoomNodeGraph != null)
        {
            // Draw Grid
            DrawBackgroundGrid(GridSmall, 0.2f, Color.black);
            DrawBackgroundGrid(GridLarge, 0.3f, Color.gray);

            // Draw line if being dragged
            DrawDraggedLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw Connections Between Room Nodes
            DrawRoomConnections();

            // Draw Room Nodes
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
            //Draw line from node to line position
            Handles.DrawBezier(CurrentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, CurrentRoomNodeGraph.linePosition, CurrentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, CurrentRoomNodeGraph.linePosition, Color.white, null, ConnectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        GraphDrag = Vector2.zero;

        // Get room node that mouse is over if it's null or not currently being dragged
        if (CurrentRoomNode == null || CurrentRoomNode.isLeftClickDragging == false)
        {
            CurrentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // if mouse isn't over a room node or we are currently dragging a line from the room node then process graph events
        if (CurrentRoomNode == null || CurrentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        // else process room node events
        else
        {
            // process room node events
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
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);

                break;

            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // Process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        // Process left mouse down on graph event
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
        // If current node graph empty then add entrance room node first
        if (CurrentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), RoomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, RoomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // add room node to current room node graph room node list
        CurrentRoomNodeGraph.roomNodeList.Add(roomNode);

        // set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(NodeWidth, NodeHeight)), CurrentRoomNodeGraph, roomNodeType);

        // add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, CurrentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        // Refresh graph node dictionary
        CurrentRoomNodeGraph.OnValidate();
    }

    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        // Loop through all nodes
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                // iterate through child room nodes ids
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // Retrieve child room node
                    RoomNodeSO childRoomNode = CurrentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null)
                    {
                        // Remove parentID from child room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                // Iterate through parent room node ids
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    // Retrieve parent node
                    RoomNodeSO parentRoomNode = CurrentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNode != null)
                    {
                        // Remove childID from parent node
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Delete queued room nodes
        while (roomNodeDeletionQueue.Count > 0)
        {
            // Get room node from queue
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            // Remove node from dictionary
            CurrentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            // Remove node from list
            CurrentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            // Remove node from Asset database
            DestroyImmediate(roomNodeToDelete, true);

            // Save asset database
            AssetDatabase.SaveAssets();

        }
    }

    private void DeleteSelectedRoomNodeLinks()
    {
        // Iterate through all room nodes
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    // Get child room node
                    RoomNodeSO childRoomNode = CurrentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    // If the child room node is selected
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        // Remove childID from parent room node
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        // Remove parentID from child room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Clear all selected room nodes
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
        // if releasing the right mouse button and currently dragging a line
        if (currentEvent.button == 1 && CurrentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            // Check if over a room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                // if so set it as a child of the parent room node if it can be added
                if (CurrentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    // Set parent ID in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(CurrentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process right click drag event - draw line
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        // process left click drag event - drag node graph
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
        // Loop through all room nodes
        foreach (RoomNodeSO roomNode in CurrentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                // Loop through child room nodes
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // get child room node from dictionary
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
        // get line start and end position
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        // calculate midway point
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // Vector from start to end position of line
        Vector2 direction = endPosition - startPosition;

        // Calulate normalised perpendicular positions from the mid point
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;

        // Calculate mid point offset position for arrow head
        Vector2 arrowHeadPoint = midPosition + direction.normalized * ConnectingLineArrowSize;

        // Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, ConnectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, ConnectingLineWidth);

        // Draw line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, ConnectingLineWidth);

        GUI.changed = true;
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes()
    {
        // Loop through all room nodes and draw themm
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

    /// <summary>
    /// Selection changed in the inspector
    /// </summary>
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