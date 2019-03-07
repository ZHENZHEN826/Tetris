using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform m_emptySprite;
    public int m_height = 30;
    public int m_width = 8;
    public int m_header = 8;

    Transform[,] m_grid;

    public int m_completeRows;

    private void Awake()
    {
        m_grid = new Transform[m_width+1, m_height];
    }


    // Start is called before the first frame update
    void Start()
    {
        DrawEmptyCells();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool IsWithinBoard(int x, int y)
    {
        return (x >= 1 && x <= m_width && y >= 0);
    }

    bool IsOccupied(int x, int y, Shape shape)
    {
        return (m_grid[x,y] != null && m_grid[x,y].parent != shape.transform);
    }

    public bool IsValidPosition(Shape shape)
    {
        foreach(Transform child in shape.transform)
        {
            Vector2 pos = Vectorf.Round(child.position);

            if (!IsWithinBoard((int)pos.x, (int)pos.y))
            {
                return false;
            }

            if (IsOccupied((int)pos.x, (int)pos.y, shape))
            {
                return false;
            }
        }

        return true;
    }

    void DrawEmptyCells()
    {
        if (m_emptySprite)
        {
            for (int y = 0; y < m_height - m_header; y++)
            {
                for (int x = 1; x <= m_width; x++)
                {
                    Transform clone;
                    clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity) as Transform;
                    clone.name = "Board Space ( x = " + x.ToString() + " , y = " + y.ToString() + " )";
                    clone.transform.parent = transform;
                }
            }
        }
        else
        {
            Debug.Log("WARNING! Please assign the emptySprite object!");
        }

    }

    public void StoreShapeInGrid(Shape shape)
    {
        if (shape == null)
        {
            return;
        }

        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vectorf.Round(child.position);
            m_grid[(int)pos.x, (int)pos.y] = child;
        }
    }

    bool IsComplete(int y)
    {
        for (int x = 1; x <= m_width; x++)
        {
            if (m_grid[x,y] == null) 
            {
                return false;
            }
        }
        return true;
    }

    void clear(int y)
    {
        for (int x = 1; x <= m_width; x++)
        {
            if (m_grid[x,y] != null)
            {
                Destroy(m_grid[x, y].gameObject);
            }
            m_grid[x, y] = null;
        }
    }

    void shiftOneRowDown(int y)
    {
        for (int x = 1; x <= m_width; x++)
        {
            if (m_grid[x,y] != null)
            {
                m_grid[x, y - 1] = m_grid[x, y];
                m_grid[x, y] = null;
                m_grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    void shiftRowsDown(int startY)
    {
        for (int i = startY; i < m_height; i++)
        {
            shiftOneRowDown(i); 
        }
    }

    public void clearAllRows()
    {
        m_completeRows = 0;
        for (int y = 0; y< m_height; y++)
        {
            if (IsComplete(y))
            {
                m_completeRows++;
                clear(y);
                shiftRowsDown(y+1);
                y--;
            }
        }
    }

    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.transform.position.y >= (m_height - m_header - 1))
            {
                return true;
            }
        }
        return false;
    }
}
