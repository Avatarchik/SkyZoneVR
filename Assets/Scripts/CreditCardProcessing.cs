using UnityEngine;
using System;
using System.Collections;
using System.Web;
using System.Security.Cryptography;
using System.Text;

public class CreditCardProcessing : MonoBehaviour {
  string outputText;
  string magStripe = "";
  string gatewayUrl = "https://www.usaepay.com/gate.php";
  public bool readyToProcess = true;

  float timeSinceLastSwipe, delay = 1f;
  void Update(){
		print (magStripe);
    if(readyToProcess){
      if(Input.inputString.Length > 0){
        magStripe = magStripe + Input.inputString;
        timeSinceLastSwipe = Time.time;
      }else if(timeSinceLastSwipe + delay < Time.time && magStripe.Length > 10){
        readyToProcess = false;
        StartCoroutine(RunSale(magStripe));
      }

			if(magStripe.Length > 1)
				GetComponent<TextManager> ().tutorialProcessingCardText.gameObject.SetActive (true);
    }
  }

  private IEnumerator RunSale(string mag){
		print ("Running sale");

		yield return new WaitForEndOfFrame();
    
    WWWForm form = new WWWForm();
		form.AddField("UMkey", "2bC7KNq072ZeT2kxA6qKSUUtbE8aEy7S");
		form.AddField("UMmagstripe", "enc://" + mag);
		form.AddField("UMamount", "3.00");
		form.AddField("UMdescription", "VRCube play!");

		// Upload to a cgi script
		WWW w = new WWW(gatewayUrl, form);
		yield return w;
    readyToProcess = true;
    magStripe = "";
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
      SendMessage("CreditCardTransaction", false);
		}
		else {
			print("Finished processed!");
      // ADD IN GAME FUNCTIONALITY HERE GREG.
      SendMessage("CreditCardTransaction", true);
		}
 
		GetComponent<TextManager> ().tutorialProcessingCardText.gameObject.SetActive (false);
  }

	public void ResetMagStripeString()
	{
		magStripe = "";
	}
}
