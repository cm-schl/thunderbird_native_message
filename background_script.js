//Listen for a click on the button in the main window
browser.browserAction.onClicked.addListener(saveMail);


function onResponse(response) {
  //Show the response from the local handler
  console.log("Received response from send_to_d3Handler.exe: " + response.result);
}

function onError(error) {
  console.log(`Error saving EML file to d.3 Folder! Check communication with send_to_d3Handler.exe. Error: ${error}`);
}


function saveMail()
{
    //Get the current open mail (in the listview or in window mode)
    promiseTab = browser.tabs.query({
        active: true,
        currentWindow: true
    })

  //Useful example: https://github.com/braineo/thunderbird-jira-opener/commit/efec97df8661cb7eade6193798d62e9e3972b010
    promiseTab.then(tabs => {

        let tabId = tabs[0].id;
        // get current opened message header
        promiseMessage = browser.messageDisplay.getDisplayedMessage(tabId);

        promiseMessage.then(function (messageHeader) 
          {
            //get the actual timestamp and the mail subject to use it as filename of the saved .eml
            let filename = messageHeader.subject;

            //use 87 chars of the subject + Date.now() (13 chars) as filename for the .eml
            filename = filename.substr(0,87) + Date.now();
            filename = filename.replace(/[^a-z0-9]/gi, '_').toLowerCase();
            //for cases where the filename is shorter than 100 chars, rpad with spaces
            filename = filename.padEnd(100);

            //get the raw content of the choosen message as string by a promise
            promiseFullMessage = browser.messages.getRaw(messageHeader.id);

            promiseFullMessage.then(function (fullMessage) 
              {
                //Only for debugging
                //console.log(fullMessage.toString());

                //write the filename to use and the raw message to a variable
                let eml_content = filename + fullMessage;
              
                console.log("Send_to_d3: Sending mail to local handler");

                //Raw EML files are ASCII encoded (see https://stackoverflow.com/questions/64558872/encounter-encoding-problem-while-copying-an-email-from-thunderbird-via-delphi-us)
                //so before sending the raw content of the mail by native messaging encode it to Base64 - otherwise the ASCII content of the mail gets interpreted as UTF-8
                //by the native messaging application. In this case all special characters in the mail afterwards can't be visualized correctly again.
                eml_content = btoa(eml_content);
                let sending = browser.runtime.sendNativeMessage("send_to_d3",eml_content);
                //React on a response from the send_to_d3Handler.exe or report error
                sending.then(onResponse, onError);
              });

          });

    });
}