using UnityEngine;

/// <summary>
/// Represents a grid of nodes.
/// </summary>
public class GridNodes
{
    private int width;
    private int height;

    private Node[,] gridNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="GridNodes"/> class with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the grid.</param>
    /// <param name="height">The height of the grid.</param>
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridNode = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// Retrieves the node at the specified position.
    /// </summary>
    /// <param name="xPosition">The x-coordinate of the node.</param>
    /// <param name="yPosition">The y-coordinate of the node.</param>
    /// <returns>The node at the specified position.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the requested position is out of range.</exception>
    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < 0 || xPosition >= width || yPosition < 0 || yPosition >= height)
        {
            throw new System.ArgumentOutOfRangeException(nameof(xPosition), "Requested grid node is out of range");
        }

        return gridNode[xPosition, yPosition];
    }
}
