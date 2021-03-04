
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CycleImage : MonoBehaviour
{
    [SerializeField] private float timeBetweenLoops = 1.5f;
    [SerializeField] private float timeBetweenFrames = 0.4f;
    [SerializeField] private float loopPause = 0.4f;

    private Image _myImage;
    [SerializeField] private Sprite[] _sprites;

    int currentIndex = 0;
    private bool _moveForward = true;

    private Coroutine _currentLoop;

    private void Start()
    {
        _myImage = this.GetComponent<Image>();

        _currentLoop = StartCoroutine(StartNextLoop());

    }

    private IEnumerator StartNextLoop()
    {
        yield return new WaitForSeconds(timeBetweenLoops);

        StartCoroutine(AnimateSpriteArray());
    }

    private IEnumerator AnimateSpriteArray()
    {
        //if (_moveForward)
        //{

        currentIndex = 0;

        while (currentIndex < _sprites.Length)
        {
            yield return new WaitForSeconds(timeBetweenFrames);
            _myImage.overrideSprite = _sprites[currentIndex];
            currentIndex += 1;
        }
        //}
        //else
        //{
        yield return new WaitForSeconds(loopPause);

        currentIndex -= 1;

        while (currentIndex > -1)
        {
            yield return new WaitForSeconds(timeBetweenFrames);
            _myImage.overrideSprite = _sprites[currentIndex];
            currentIndex -= 1;
        }
        //}

        _moveForward = !_moveForward;

        StartCoroutine(StartNextLoop());
    }
}

