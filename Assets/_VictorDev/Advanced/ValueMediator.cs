using _VictorDev.MathUtils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Debug = _VictorDev.DebugUtils.Debug;

namespace _VictorDev.Frameworks
{
    /// [Mediator] - 數值轉接器
    public class ValueMediator : MonoBehaviour
    {
        public void SetString(string stringValue)
        {
            if (float.TryParse(stringValue, out float floatResult))
            {
                SetValue(floatResult);
            }
            else
            {
                DebugUtils.Debug.Log($"字串[{stringValue}]無法轉成float值");
            }
        }

        public void SetValue(float value)
        {
            _value = value;
            InvokeValueHandler();
        }

        public void SetValueWithMultiply(float value) => SetValue(value * mutiplyValue);

        private void InvokeValueHandler()
        {
            invokeString?.Invoke(MathHelper.ToDotNumberString(_value, dotNumber));
            invokeFloat?.Invoke(MathHelper.ToDotNumberFloat(_value, dotNumber));
            invokeFloat01?.Invoke(MathHelper.ToPercent01(_value, maxValue, dotNumber));
            invokeInteger?.Invoke(Mathf.RoundToInt(_value));
        }

        #region Variabls

        [Foldout("發送字串")] public UnityEvent<string> invokeString;
        [Foldout("發送float")] public UnityEvent<float> invokeFloat;
        [Foldout("發送float01")] public UnityEvent<float> invokeFloat01;
        [Foldout("發送Integer")] public UnityEvent<float> invokeInteger;

        [Foldout("[設定]")] [SerializeField] private int dotNumber;
        [Foldout("[設定]")] [SerializeField] private int maxValue;
        [Foldout("[設定]")] [SerializeField] private int mutiplyValue = 1;
        private float _value;

        #endregion


        #region 測試

        [Foldout("[測試]")] [SerializeField] private float testValue;

        [Button]
        private void TestValue() => SetValue(testValue);
        [Button]
        private void TestRandomValue() => SetValue(Random.Range(1, testValue));

        #endregion
    }
}