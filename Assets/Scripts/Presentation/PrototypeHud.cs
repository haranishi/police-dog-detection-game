using System;
using UnityEngine;

namespace PoliceDog.Presentation
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        private string headline = "到着ロビー訓練";
        private string message = "Shift長押しで匂いに集中し、Spaceで座って知らせる";
        private string detail = "WASD 移動 / Ctrl 走る / R 鼻の高さ / Shift 嗅覚 / Space 通知";
        private bool rewardAvailable;
        private Action rewardAction;
        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle textStyle;
        private GUIStyle buttonStyle;

        public void SetStatus(string newHeadline, string newMessage, string newDetail = "")
        {
            headline = newHeadline;
            message = newMessage;
            detail = newDetail;
            rewardAvailable = false;
            rewardAction = null;
        }

        public void OfferReward(Action onAccepted)
        {
            rewardAvailable = true;
            rewardAction = onAccepted;
        }

        private void OnGUI()
        {
            EnsureStyles();
            GUI.Box(new Rect(18, 18, 560, rewardAvailable ? 190 : 150), GUIContent.none, boxStyle);
            GUI.Label(new Rect(36, 32, 520, 30), headline, titleStyle);
            GUI.Label(new Rect(36, 69, 520, 46), message, textStyle);
            GUI.Label(new Rect(36, 117, 520, 30), detail, textStyle);
            if (rewardAvailable && GUI.Button(new Rect(36, 148, 210, 32), "玩具で褒めてもらう", buttonStyle))
            {
                rewardAction?.Invoke();
            }

            GUI.Box(new Rect(Screen.width - 300, 18, 282, 118), GUIContent.none, boxStyle);
            GUI.Label(new Rect(Screen.width - 282, 30, 250, 28), "匂いの文法", titleStyle);
            GUI.Label(new Rect(Screen.width - 282, 63, 250, 65),
                "▮ 細い連続束 = 対象\n● 広がる丸 = 妨害\n- - 途切れた線 = 残留", textStyle);
        }

        private void EnsureStyles()
        {
            if (boxStyle != null) return;
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = MakeTexture(new Color(0.04f, 0.07f, 0.1f, 0.92f));
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
            titleStyle.normal.textColor = Color.white;
            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 15, wordWrap = true };
            textStyle.normal.textColor = new Color(0.9f, 0.95f, 1f);
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 15, fontStyle = FontStyle.Bold };
        }

        private static Texture2D MakeTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
