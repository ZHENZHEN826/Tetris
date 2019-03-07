 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{

    public Transform m_holderXform;
    public Shape m_heldShape = null;
    float m_scale = 0.5f;
    public bool m_canRelease = false;

    public void Catch(Shape shape)
    {
        if (m_heldShape)
        {
            Debug.LogWarning("HOLDER WARNING! Release a shape before trying to hold!");
            return;
        }

        if (!shape)
        {
            Debug.LogWarning("HOLDER WARNING! Invalid shape!");
            return;
        }

        if (m_holderXform)
        {
            shape.transform.position = m_holderXform.position + shape.m_queueOffset;
            shape.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
            //shape.transform.eulerAngles = Vector3.zero;
            shape.transform.rotation = Quaternion.identity;
            m_heldShape = shape;
        }
        else
        {
            Debug.LogWarning("HOLDER WARNING! Holder has no transform assigned!");
        }
    }
   
    public Shape Release()
    {
        m_heldShape.transform.localScale = Vector3.one;
        Shape shape = m_heldShape;
        m_heldShape = null;

        m_canRelease = false;

        return shape;
    }
}
