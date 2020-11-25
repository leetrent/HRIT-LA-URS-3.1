function confirmEmailAddress(userId) {

    let logSnippet = "[user_confirm_email.js][confirmEmailAddress] => ";

    console.log(`${logSnippet}(userId): '${userId}'`);

    const userAccount = {
        id: userId
    };

    console.log(`${logSnippet}(userAccount):`, userAccount);

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: "/api/user/emailaddress",
        contentType: "application/json",
        data: JSON.stringify(userAccount),
        error: function (jqXHR, testStatus, errorThrown) {
            console.log(`${logSnippet}[error] => (jqXHR)......:`, jqXHR);
            console.log(`${logSnippet}[error] => (testStatus).:`, testStatus);
            console.log(`${logSnippet}[error] => (errorThrown):`, errorThrown);
        },
        success: function (result) {
            console.log(`${logSnippet}[success] => (result):`, result);
            isEmailAddressConfirmed(userId);
        }
    });
}

function isEmailAddressConfirmed(userId) {

    const logSnippet = "[user_confirm_email.js][isEmailAddressConfirmed] => ";
    console.log(`${logSnippet} (userId): ${userId}`);

    $.ajax({
        type: "GET",
        url: "/api/user/emailaddress",
        data: {
            userId: userId
        },
        cache: false,
        success: function (data) {
            console.log(`${logSnippet}[success] => (data): ${data}`);
            processIsEmailAddressConfirmedResponse(data);

        }
    });
}

function processIsEmailAddressConfirmedResponse(responseData) {

    let emailAddressIsConfirmed = document.getElementById("emailAddressIsConfirmed");
    if (emailAddressIsConfirmed) {
        emailAddressIsConfirmed.innerText = responseData;
        emailAddressIsConfirmed.style.backgroundColor = "rgb(40, 167, 69, 0.30)";
        emailAddressIsConfirmed.setAttribute("class", "border border-success");
    }

    processEmailConfirmedCard();
    processEmailConfirmedPopup();
}

function processEmailConfirmedCard() {

    let confirmEmailCard = document.getElementById("confirmEmailCard");
    if (confirmEmailCard) {
        confirmEmailCard.setAttribute("class", "card bg-light text-secondary");
    }

    let confirmEmailButton = document.getElementById("confirmEmailButton");
    if (confirmEmailButton) {
        confirmEmailButton.setAttribute("class", "btn btn-secondary");
        confirmEmailButton.disabled = true;
    }
}

function processEmailConfirmedPopup() {

    let userAccountModalTitle = document.getElementById("userAccountModalTitle");
    if (userAccountModalTitle) {
        userAccountModalTitle.innerText = "Email Confirmed";
    }

    let userAccountModelBody = document.getElementById("userAccountModelBody");
    if (userAccountModelBody) {
        userAccountModelBody.innerText = "This user's email address has been confirmed.";
    }

    let userAccountModalButton = document.getElementById("userAccountModalButton");
    if (userAccountModalButton) {
        userAccountModalButton.click();
    }
}