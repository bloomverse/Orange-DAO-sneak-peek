
mergeInto(LibraryManager.library, {
  GetTextFromJavaScript: function() {
    var text = prompt("Please enter the text:", "");
    // Ensure 'GameObjectName' matches the name of the GameObject in Unity that has the TextInputHandler script
    window.unityGame.sendMessage('gameInstance', 'ReceiveTextFromJavaScript', text);
  }
});