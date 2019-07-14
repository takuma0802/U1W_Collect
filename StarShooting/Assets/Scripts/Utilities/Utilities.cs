using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities {
    // 移動可能な範囲
    public static Vector2 m_moveLimit = new Vector2 (5.7f, 3.7f);

    // 画面に映る範囲
    public static Vector2 m_visibleLimit = new Vector2 (8f, 6f);

    // 指定された位置を移動可能な範囲に収めた値を返す
    public static Vector3 ClampPosition (Vector3 position) {
        return new Vector3 (
            Mathf.Clamp (position.x, -m_moveLimit.x, m_moveLimit.x),
            Mathf.Clamp (position.y, -m_moveLimit.y, m_moveLimit.y),
            0
        );
    }

    // 指定された位置が画面内かどうかを返す
    public static bool CheckVisibllity (Vector3 position) {
        var checkX = position.x > -m_visibleLimit.x && position.x < m_visibleLimit.x;
        var checkY = position.y > -m_visibleLimit.y && position.y < m_visibleLimit.y;
        return checkX && checkY;
    }

    public static float GetAngle (Vector2 from, Vector2 to) {
        var dx = to.x - from.x;
        var dy = to.y - from.y;
        var rad = Mathf.Atan2 (dy, dx);
        return rad * Mathf.Rad2Deg;
    }

    // 指定された角度（ 0 ～ 360 ）をベクトルに変換して返す
    public static Vector3 GetDirection (float angle) {
        return new Vector3 (
            Mathf.Cos (angle * Mathf.Deg2Rad),
            Mathf.Sin (angle * Mathf.Deg2Rad),
            0
        );
    }

    public static TEnum ConvertToEnum<TEnum> (int number) {
        return (TEnum) Enum.ToObject (typeof (TEnum), number);
    }

    public static Tweener DOTextInt (this Text text, int initialValue, int finalValue, float duration, Func<int, string> convertor) {
        return DOTween.To (
            () => initialValue,
            it => text.text = convertor (it),
            finalValue,
            duration
        );
    }

    public static Tweener DOTextInt (this Text text, int initialValue, int finalValue, float duration) {
        return Utilities.DOTextInt (text, initialValue, finalValue, duration, it => it.ToString ());
    }
}