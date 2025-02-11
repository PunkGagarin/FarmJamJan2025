using UnityEngine;

[System.Serializable]
public class CustomKeyValue<K, V> {

    [SerializeField]
    private K _key;

    [SerializeField]
    private V _value;

    public CustomKeyValue() {
    }

    public CustomKeyValue(K key) {
        _key = key;
    }

    public CustomKeyValue(V value) {
        _value = value;
    }

    public CustomKeyValue(K key, V value) {
        _key = key;
        _value = value;
    }

    public K key {
        get => _key;
        set => _key = value;
    }

    public V value {
        get => _value;
        set => _value = value;
    }
}