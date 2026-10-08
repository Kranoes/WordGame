using System;
using System.Collections.Generic;
using UnityEngine;

namespace GuessWordGame
{
    public static class PromoCodeManager
    {
        private class PromoCodeInfo
        {
            public string RewardText { get; }
            public Action<GameData> Action { get; }

            public PromoCodeInfo(string rewardText, Action<GameData> action)
            {
                RewardText = rewardText;
                Action = action;
            }
        }

        private static readonly Dictionary<string, PromoCodeInfo> PromoCodes = new Dictionary<string, PromoCodeInfo>
        {
            { "START", new PromoCodeInfo("Получено +500 монет!", data => data.coins += 500) },
            { "RUBIES", new PromoCodeInfo("Получено +50 рубинов!", data => data.rubies += 50) },
            { "GUESS2026", new PromoCodeInfo("Получено +1000 монет и +100 рубинов!", data => { data.coins += 1000; data.rubies += 100; }) }
        };

        public static bool TryRedeemCode(string code, out string message)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                message = "Введите промокод";
                return false;
            }

            string upperCode = code.Trim().ToUpper();

            GameData data = SaveManager.CurrentData;
            if (data == null)
            {
                message = "Ошибка загрузки данных";
                return false;
            }

            // Защита от NRE и проверка по корректному полю redeemedPromoCodes
            if (data.redeemedPromoCodes != null && data.redeemedPromoCodes.Contains(upperCode))
            {
                message = "Промокод уже использован";
                return false;
            }

            if (PromoCodes.TryGetValue(upperCode, out PromoCodeInfo promoInfo))
            {
                promoInfo.Action?.Invoke(data);

                if (data.redeemedPromoCodes == null)
                {
                    data.redeemedPromoCodes = new List<string>();
                }
                data.redeemedPromoCodes.Add(upperCode);

                // Записываем данные в файл/PlayerPrefs и перерисовываем баланс на экране
                SaveManager.Save();
                UIManager.Instance?.UpdateCurrencyUI();

                message = promoInfo.RewardText;
                return true;
            }

            message = "Неверный промокод";
            return false;
        }
    }
}