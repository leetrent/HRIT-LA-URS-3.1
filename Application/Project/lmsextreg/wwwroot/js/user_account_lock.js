function unlockUserAccount(userId) {
    console.log(`[user_unlock.js][unlockUserAccount] => (userId): ${userId}`)

    const userAccount = {
        id: userId
    };

    console.log("`[user_unlock.js][unlockUserAccount] => (userAccount): ", userAccount);

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: "/api/user/account/lock",
        contentType: "application/json",
        data: JSON.stringify(userAccount),
        error: function (jqXHR, testStatus, errorThrown) {
            console.log("[user_unlock.js][unlockUserAccount][error] => (jqXHR): ", jqXHR);
            console.log("[user_unlock.js][unlockUserAccount][error] => (testStatus): ", testStatus);
            console.log("[user_unlock.js][unlockUserAccount][error] => (errorThrown): ", errorThrown);
        },
        success: function (result) {
            console.log("[user_unlock.js][unlockUserAccount][success] => (result): ", result);
            isUserAccountLocked(userId);
        }
    });
}

function isUserAccountLocked(userId) {
    console.log(`[user_unlock.js][isUserAccountLocked] => (userId): ${userId}`);

    $.ajax({
        type: "GET",
        url: "/api/user/account/lock",
        data: {
            userId: userId
        },
        cache: false,
        success: function (data) {
            console.log(`[user_unlock.js][isUserAccountLocked][success] => (data): ${data}`);
            processResponse(data);

         }
    });
}

function processResponse(responseData) {

    let userIsLockedOut = document.getElementById("userIsLockedOut");
    if (userIsLockedOut) {
        userIsLockedOut.innerText = responseData;
        userIsLockedOut.style.backgroundColor = "rgb(40, 167, 69, 0.30)";
        userIsLockedOut.setAttribute("class", "border border-success");
    }

    processCard();
    processPopup();
}

function processCard() {

    let unlockAccountCard = document.getElementById("unlockAccountCard");
    if (unlockAccountCard) {   
        unlockAccountCard.setAttribute("class", "card bg-light text-secondary");
    }

    let unlockAccountButton = document.getElementById("unlockAccountButton");
    if (unlockAccountButton) {
        unlockAccountButton.setAttribute("class", "btn btn-secondary");
        unlockAccountButton.disabled = true;
    }
}


function processPopup() {

    let userAccountModalTitle = document.getElementById("userAccountModalTitle");
    if (userAccountModalTitle) {
        userAccountModalTitle.innerText = "Account Unlocked";
    }

    let userAccountModelBody = document.getElementById("userAccountModelBody");
    if (userAccountModelBody) {
        userAccountModelBody.innerText = "This user's account has been unlocked.";
    }

    let userAccountModalButton = document.getElementById("userAccountModalButton");
    if (userAccountModalButton) {
        userAccountModalButton.click();
    }
}