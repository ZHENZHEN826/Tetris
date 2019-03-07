using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    Shape m_ghostShape = null;
    bool m_hitButtom = false;
    public Color m_color = new Color(1f, 1f, 1f, 0.2f);

    public void DrawGhost(Shape originalShape, Board gameBoard)
    {
        if (!m_ghostShape)
        {
            m_ghostShape = Instantiate(originalShape, transform.transform.position, originalShape.transform.rotation) as Shape;
            m_ghostShape.gameObject.name = "GhostName";

            SpriteRenderer[] allRenderers = m_ghostShape.GetComponentsInChildren<SpriteRenderer>();

            foreach(SpriteRenderer r in allRenderers)
            {
                r.color = m_color;
            }
        }
        else
        {
            m_ghostShape.transform.position = originalShape.transform.position;
            m_ghostShape.transform.rotation = originalShape.transform.rotation;
        }

        m_hitButtom = false;

        while (!m_hitButtom)
        {
            m_ghostShape.MoveDown();
            if (!gameBoard.IsValidPosition(m_ghostShape))
            {
                m_ghostShape.MoveUp();
                m_hitButtom = true;
            }
        }
    }

    public void Reset()
    {
        Destroy(m_ghostShape.gameObject);
    }
}
