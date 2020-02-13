using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour
{
    public float power = 2;
    public int dmgType;

    private Animator anim;
    public GameObject colObject;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        RotateToMouse();

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("meleeAttack");
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<ResourceDrop>(out ResourceDrop resourceScript))
        {

            float dmg = (dmgType != 0 && dmgType == resourceScript.resourceType) ? power * 2 : power;

            resourceScript.DamageResource(dmg);
            colObject.SetActive(false);
        }
    }

    void RotateToMouse()
    {
        if (Camera.main == null)
            return;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x -= objectPos.x;
        mousePos.y -= objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
