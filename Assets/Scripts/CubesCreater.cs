using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class CubesCreater : MonoBehaviour
{
    [SerializeField] private GameObject _cube;
    [SerializeField] private Transform _minPosition;
    [SerializeField] private Transform _maxPosition;

    private int _poolCapacity = 20;
    private int _poolMaxSize = 20;
    private ObjectPool<GameObject> _pool;
    private Material _material;
    private float _repeatRate = 1f;
    private float _minLifeTime = 2f;
    private float _maxLifeTime = 5f;

    private void Awake()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(_cube),
            actionOnGet: (cube) => ActionOnGet(cube),
            actionOnRelease: (cube) => cube.SetActive(false),
            actionOnDestroy: (cube) => Destroy(cube),
            collectionCheck: false,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize);

        _material = _cube.GetComponent<Renderer>().sharedMaterial;
    }

    private void ActionOnGet(GameObject cube)
    {
        cube.transform.position = new Vector3(Random.Range(_minPosition.position.x, _maxPosition.position.x), _minPosition.position.y,
                                              Random.Range(_minPosition.position.z, _maxPosition.position.z));
        cube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cube.GetComponent<Renderer>().material = _material;

        if (cube.TryGetComponent<Cube>(out Cube cubeComponent))
            cubeComponent.ChangeTouch();

        cube.SetActive(true);
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0.0f, _repeatRate);
    }

    private void GetCube()
    {
        if (_pool.CountAll < _poolMaxSize || _pool.CountInactive > 0)
            _pool.Get();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float lifeTime = Random.Range(_minLifeTime, _maxLifeTime);
        StartCoroutine(WaitOnRelease(collision.gameObject, lifeTime));
    }

    private IEnumerator WaitOnRelease(GameObject gameObject, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        _pool.Release(gameObject);
        yield break;
    }
}
