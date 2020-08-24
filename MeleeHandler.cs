using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

public class MeleeHandler : MonoBehaviour
{
    InvenManager2 invenMngr;

    public float power = 2;
    public float powerBonus = 0;
    public int dmgType = 0;

    private Animator anim;
    public GameObject colObject;

    private void Start()
    {
        invenMngr = InvenManager2.invenManager2;

        anim = GetComponent<Animator>();
        invenMngr.OnMainhandChange += UpdateMainhand;
        // Run update functions in start to make sure values are initialized correctly
        UpdateMainhand();
    }

    private void Update()
    {
        RotateToMouse();

        if (Input.GetMouseButtonDown(0))
        {
            if (dmgType != 5)
                anim.SetTrigger("meleeAttack");
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<ResourceDrop>(out ResourceDrop resourceScript))
        {

            float dmg = (dmgType == resourceScript.resourceType) ? power : power * 0.1f;

            resourceScript.DamageResource(dmg);
            colObject.SetActive(false);
        }
    }

    // --- EVENT METHODS ---

    // Update player base attack power and dmg type whenever the OnMainhandChange event is fired in InventoryManager
    void UpdateMainhand ()
    {
        Item item = ItemsDatabase.itemsDatabase.GetItem(invenMngr.Equipment[3].Item.ID);
        if (item is Mainhand mainhand)
        {
            power = mainhand.Power;
            dmgType = mainhand.DmgType;
        }
        else
        {
            // Empty hands
            power = 1;
            dmgType = 0;
        }
    }
    // Update player bonus power when the OnPassiveSkillChange event is fired in SkillManager
    void UpdatePowerBonus ()
    {
        if (SkillManager.skillManager.activeSkills.TryGetValue("Strength", out int level))
        {
            powerBonus = 0.1f * level;
        }
    }

    // --- UTILITIES ---
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