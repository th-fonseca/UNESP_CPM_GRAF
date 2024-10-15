using UnityEngine;
using UnityEditor;

public class TagAssigner : MonoBehaviour
{
    [MenuItem("Tools/Apply Tag to Children")]
    static void ApplyTagToChildren()
    {
        // Verifica se algum objeto está selecionado
        if (Selection.activeGameObject != null)
        {
            Transform parent = Selection.activeGameObject.transform;
            string tagToApply = parent.tag;

            // Aplica a tag do pai em todos os filhos
            ApplyTagRecursively(parent, tagToApply);
            Debug.Log("Tag aplicada a todos os filhos de " + parent.name);
        }
        else
        {
            Debug.LogError("Nenhum objeto selecionado.");
        }
    }

    static void ApplyTagRecursively(Transform parent, string tagToApply)
    {
        // Aplica a tag no objeto pai
        parent.tag = tagToApply;

        // Percorre cada filho e aplica a tag
        foreach (Transform child in parent)
        {
            child.tag = tagToApply;
            ApplyTagRecursively(child, tagToApply); // Chama recursivamente para garantir que todos os subfilhos também recebam a tag
        }
    }
}
