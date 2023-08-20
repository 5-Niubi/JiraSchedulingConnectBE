// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const EMAIL = "area1110@outlook.com";

// Write your JavaScript code.
function paging(totalPage, pageNum) {
	let gap = 2;
	let pagingContent = "";

	let url = new URL(location.href);
	let params = url.searchParams;
	//let pageNum;
	//if (params.has("pageNum")) {
	//    pageNum = Number(params.get("pageNum"));
	//} else {
	//    pageNum = 1;
	//}

	if (pageNum > 1) {
		params.set("pageNum", (Number(pageNum) - 1).toString());
		pagingContent += `<li class="page-item">
                        <a class="page-link" href="${url}" tabindex="-1">Previous</a>
                        </li>`;
	}
	let pageFront = pageNum - gap;
	if (pageFront < 1) {
		pageFront = 1;
	}
	for (let i = pageFront; i < pageNum; i++) {
		params.set("pageNum", i.toString());
		pagingContent += `<li class="page-item"><a class="page-link" href="${url}">${i}</a></li>`;
	}

	// print current page;
	pagingContent += `<li class="page-item active" aria-current="page">
      <a class="page-link" href="#">${pageNum}</a>
    </li>`;

	let pageRear = pageNum + gap;
	if (pageRear > totalPage) {
		pageRear = totalPage;
	}
	for (let i = pageNum + 1; i <= pageRear; i++) {
		params.set("pageNum", i);

		pagingContent += `<li class="page-item"><a class="page-link" href="${url}">${i}</a></li>`;
	}

	if (pageNum < totalPage) {
		params.set("pageNum", (Number(pageNum) + 1).toString());
		pagingContent += `<li class="page-item">
      <a class="page-link" href="${url}">Next</a>
    </li>`;
	}

	$("#pagination").html(pagingContent);
}

function imageToBase64(inputDomId) {
	const imageInput = document.getElementById(inputDomId);

	const file = imageInput?.files[0];
	if (!file) {
		alert("Please select an image.");
		return;
	}

	const reader = new FileReader();
	let result;
	reader.onload = function () {
		const base64 = reader.result;
		result = base64;
	};

	reader.readAsDataURL(file);
	return result;
}

function mailTo() {
	let emailTo = "area1110@outlook.com";

	let userCode = $("#userToken").val();
	let description = $("#description").val();

	let subject = `[WoTaas][Support] ${userCode}`;
	let body = `
        UserCode: ${userCode}
        TransferCode: ${transferCode}
        Description: ${description}
    `;

	subject = encodeURIComponent(subject);
	body = encodeURIComponent(body);

	let urlMail = new URL(`mailto:${emailTo}?subject=${subject}&body=${body}`);

	const emailWindow = window.open(urlMail, "_blank");
	// Close the window after a short delay (optional)
	setTimeout(() => {
		emailWindow?.close();
	}, 500);
}

function getExchangeRate() {
	// Load exchange rates data via AJAX:
	$.getJSON(
		// NB: using Open Exchange Rates here, but you can use any source!
		"https://openexchangerates.org/api/latest.json?app_id=[YOUR APP ID]",
		function (data) {
			// Check money.js has finished loading:
			if (typeof fx !== "undefined" && fx.rates) {
				fx.rates = data.rates;
				fx.base = data.base;
			} else {
				// If not, apply to fxSetup global:
				var fxSetup = {
					rates: data.rates,
					base: data.base,
				};
			}
		}
	);
}

function setExchangeRate() {
	fx.base = "USD";
	fx.rates = {
		EUR: 0.745101, // eg. 1 USD === 0.745101 EUR
		GBP: 0.64771, // etc...
		HKD: 7.781919,
		USD: 1, // always include the base rate (1:1)
		/* etc */
		VND: 23500,
	};
}

function convertToVND(amount) {
	setExchangeRate();
	return fx.convert(amount, { from: "USD", to: "VND" });
}

function vietQrQuicklinkBuilder(amount, token, planName, contact) {
	const BANK_INFO = {
		bankId: "VCCB",
		accountNo: "9037041035051",
		template: "compact2",
	};
	let bankInfo = BANK_INFO;

	let tokenInfo = "[User Token]";
	if (token || token.length > 0) {
		tokenInfo = token;
	}
	let planInfo = "[Gói nâng cấp]";
	if (planName || planName.length > 0) {
		planInfo = planName;
	}
	let contacInfo = "[thongtinlienhe]";
    if(contact || contact.length > 0){
        contacInfo = contact
    }

	let transferInfo = {
		amount: convertToVND(amount),
		addInfo: `${planInfo} ${tokenInfo} ${contacInfo}`,
		accountName: "Nguyen Khanh Huy",
	};

	let linkElements = [];
	linkElements.push(`https://img.vietqr.io/image/`);
	linkElements.push(
		`${bankInfo.bankId}-${bankInfo.accountNo}-${bankInfo.template}`
	);
	linkElements.push(`.png?`);

	let url = new URL(linkElements.join(""));
	url.searchParams.set("amount", transferInfo.amount);
	url.searchParams.set("addInfo", transferInfo.addInfo);
	url.searchParams.set("accountName", transferInfo.accountName);

	return url.toString();
}

function checkOut(anchorTag, event) {
	event.preventDefault();
	let currentUrl = new URL(window.location.href);

	let href = anchorTag.getAttribute("href");
	let planId = anchorTag.getAttribute("data-plan");
	let params = { token: currentUrl.searchParams.get("token"), plan: planId };

	let inputElements = "";
	for (let key in params) {
		inputElements += `<input type="hidden" name="${key}" value="${params[key]}" />`;
	}
	$("body").append(
		`<form action="${href}" method="post" id="goToCheckOut">${inputElements}</form>`
	);
	$("#goToCheckOut").submit();
}
