using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public GameObject[] pages;         
    public GameObject rightArrow;      
    public GameObject leftArrow;      
    public GameObject menu;           
    public GameObject tutorialPanel;  

    private int currentPage = 0;      

    void Start()
    {
        ShowPage(currentPage);         
        UpdateArrows();                
    }

    void ShowPage(int pageIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == pageIndex); 
        }
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)    
        {
            currentPage++;
            ShowPage(currentPage);              
            UpdateArrows();                     
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)                    
        {
            currentPage--;
            ShowPage(currentPage);              
            UpdateArrows();                     
        }
        else
        {
            OpenMenu();                         
        }
    }

    void OpenMenu()
    {
        tutorialPanel.SetActive(false);    
        menu.SetActive(true);                  
    }

    void UpdateArrows()
    {
        rightArrow.SetActive(currentPage < pages.Length - 1);
        leftArrow.SetActive(true);                            
    }
}
