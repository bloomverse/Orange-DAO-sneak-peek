mergeInto(LibraryManager.library, {
    ClickPhantom: function () {
        window.dispatchReactUnityEvent("ClickPhantom");
    },
    CheckIfPhantom: function () {
        window.dispatchReactUnityEvent("CheckIfPhantom");
    },
    CheckIfParticle: function () {
        window.dispatchReactUnityEvent("CheckIfParticle");
    },
    ClickTokenMarket: function () {
        window.dispatchReactUnityEvent("TokenMarket");
    },
    bloomieTransaction: function (amount,tokenId,tokenName) {
        window.dispatchReactUnityEvent("bloomieTransaction",amount,Pointer_stringify(tokenId),Pointer_stringify(tokenName));
    },
    solanaTransaction: function (solPrice,bloomiesAmount,userWallet) {
        window.dispatchReactUnityEvent("solanaTransaction",solPrice,bloomiesAmount,Pointer_stringify(userWallet));
    },
    ClickGoogle: function () {
        window.dispatchReactUnityEvent("ClickGoogle");
    },
    OnGoogleLogin: function () {
        window.dispatchReactUnityEvent("OnGoogleLogin");
    },
    OnGoogleLoginError: function () {
        window.dispatchReactUnityEvent("OnGoogleLoginError");
    },
    OpenURL: function (url) {
		
	console.log("opennign",url);
	window.dispatchReactUnityEvent("OpenURL",Pointer_stringify(url));
	//window.open(UTF8ToString(url), "_blank");
    },
    ChangeBrowserURL: function(params){
        window.history.replaceState(null,document.title,params);
    },
    ClickParticle: function () {
	window.dispatchReactUnityEvent("ClickParticle");
	
    },
    OpenURLInSameTab: function (url) {
		window.open(UTF8ToString(url), "_self");
    },
    PasteHereWindow: function (sometext) {
    	var pastedtext= prompt("Paste Ready Player Me URL:", "Ready Player Me URL");
	window.dispatchReactUnityEvent("PasteText",pastedtext);
   },
   CopyToClipboard: function(textPointer) {
    // Convert the pointer to a string
    var text = UTF8ToString(textPointer);
    
    // Use the Clipboard API or fallback to document.execCommand for older browsers
    if (navigator.clipboard && navigator.clipboard.writeText) {
      navigator.clipboard.writeText(text).then(function() {
        console.log('Text copied to clipboard successfully!');
      }).catch(function(err) {
        console.error('Failed to copy text: ', err);
      });
    } else if (document.queryCommandSupported && document.queryCommandSupported('copy')) {
      var textarea = document.createElement('textarea');
      textarea.textContent = text;
      textarea.style.position = 'fixed';  // Prevent scrolling to bottom of page in MS Edge.
      document.body.appendChild(textarea);
      textarea.select();
      try {
        document.execCommand('copy');  // Security exception may be thrown by some browsers.
        console.log('Text copied to clipboard successfully!');
      } catch (ex) {
        console.warn('Copy to clipboard failed.', ex);
      } finally {
        document.body.removeChild(textarea);
      }
    }
    },
    ShowReadyPlayerMeFrame: function () {
        var rpmContainer = document.getElementById("rpm-container");
        rpmContainer.style.display = "block";
    },
  
    HideReadyPlayerMeFrame: function () {
        var rpmContainer = document.getElementById("rpm-container");
        rpmContainer.style.display = "none";
    },
        
    SetupRpm: function (partner){
	window.dispatchReactUnityEvent("setupRpmFrame",UTF8ToString(partner));
        //setupRpmFrame(UTF8ToString(partner));
    },
});

