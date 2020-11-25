function disableTwoFactorAuth(userId) {

    let logSnippet = "[user_disable_two_factor.js][disableTwoFactorAuth] => ";

    console.log(`${logSnippet}(userId): '${userId}'`);

    const userAccount = {
        id: userId
    };

    console.log(`${logSnippet}(userAccount):`, userAccount);

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: "/api/user/twofactorauth",
        contentType: "application/json",
        data: JSON.stringify(userAccount),
        error: function (jqXHR, testStatus, errorThrown) {
            console.log(`${logSnippet}[error] => (jqXHR)......:`, jqXHR);
            console.log(`${logSnippet}[error] => (testStatus).:`, testStatus);
            console.log(`${logSnippet}[error] => (errorThrown):`, errorThrown);
        },
        success: function (result) {
            console.log(`${logSnippet}[success] => (result):`, result);
            isTwoFactorAuthEnabled(userId);
        }
    });
}

function isTwoFactorAuthEnabled(userId) {

    const logSnippet = "[user_disable_two_factor.js][isTwoFactorAuthEnabled] => ";
    console.log(`${logSnippet} (userId): ${userId}`);

    $.ajax({
        type: "GET",
        url: "/api/user/twofactorauth",
        data: {
            userId: userId
        },
        cache: false,
        success: function (data) {
            console.log(`${logSnippet}[success] => (data): ${data}`);
            processIsTwoFactorAuthEnabledResponse(data);
        }
    });
}

function processIsTwoFactorAuthEnabledResponse(responseData) {

    let twoFactorEnabled = document.getElementById("twoFactorEnabled");
    if (twoFactorEnabled) {
        twoFactorEnabled.innerText = responseData;
        twoFactorEnabled.style.backgroundColor = "rgb(40, 167, 69, 0.30)";
        twoFactorEnabled.setAttribute("class", "border border-success");
    }

    processTwoFactorAuthCard();
    processTwoFactorAuthDisabledPopup();
}



function processTwoFactorAuthCard() {

    let disableTwoFactorCard = document.getElementById("disableTwoFactorCard");
    if (disableTwoFactorCard) {
        disableTwoFactorCard.setAttribute("class", "card bg-light text-secondary")
    }

    let disableTwoFactorButton = document.getElementById("disableTwoFactorButton");
    if (disableTwoFactorButton) {
        disableTwoFactorButton.setAttribute("class", "btn btn-secondary");
        disableTwoFactorButton.disabled = true;
    }
}

function processTwoFactorAuthDisabledPopup() {

    let userAccountModalTitle = document.getElementById("userAccountModalTitle");
    if (userAccountModalTitle) {
        userAccountModalTitle.innerText = "Two-Factor Authentication Disabled";
    }

    let userAccountModelBody = document.getElementById("userAccountModelBody");
    if (userAccountModelBody) {
        userAccountModelBody.innerText = "Two-factor authentication for this user has been disabled.";
    }

    let userAccountModalButton = document.getElementById("userAccountModalButton");
    if (userAccountModalButton) {
        userAccountModalButton.click();
    }
}