using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AI_Package
{
    public class AttackedScrollingText : MonoBehaviour
    {
        public ScrollingText Text;
        public Color color;

        public void Attacked(GameObject attacker, Attack attack)
        {
            var text = attack.damage.ToString();

            var scrollingText = Instantiate(Text, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            scrollingText.SetText(text);
            scrollingText.SetColor(color);
        }
    }
}