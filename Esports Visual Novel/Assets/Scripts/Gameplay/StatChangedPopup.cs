using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatChangedPopup : MonoBehaviour
{
    // How long this popup will show for before going away.
    private const float Lifetime = 3.0f;

    // The popup's text component
    [SerializeField] private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HideAfterDelay());
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(Lifetime);

        Destroy(gameObject);
    }
}
