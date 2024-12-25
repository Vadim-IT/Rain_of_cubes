using UnityEngine;

public class Cube : MonoBehaviour
{
    private bool _hasTouch = false;

    public void ChangeTouch() 
        => _hasTouch = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouch == false)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = Random.ColorHSV();
            GetComponent<Renderer>().material.color = material.color;
            _hasTouch = true;
        }
    }
}
