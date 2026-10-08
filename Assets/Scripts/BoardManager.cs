using UnityEngine;

namespace GuessWordGame
{
    public class BoardManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cellsParent;

        [Header("Settings")]
        [SerializeField] private int rows = 6;
        [SerializeField] private int cols = 5;

        private void Start()
        {
            CreateBoard();
        }

        public void CreateBoard()
        {
            foreach (Transform child in cellsParent)
            {
                Destroy(child.gameObject);
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GameObject cellObj = Instantiate(cellPrefab, cellsParent);
                    Cell cell = cellObj.GetComponent<Cell>();

                    if (cell != null)
                    {
                        cell.Initialize(r, c);
                        GameManager.Instance.RegisterCell(cell, r, c);
                    }
                    else
                    {
                        Debug.LogError("На префабе Cell отсутствует компонент Cell!");
                    }
                }
            }

            GameManager.Instance?.OnBoardCreated();
        }
    }
}