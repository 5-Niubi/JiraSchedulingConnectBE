// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

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
        params.set("pageNum", pageNum - 1);
        pagingContent += `<li class="page-item">
                        <a class="page-link" href="${url}" tabindex="-1">Previous</a>
                        </li>`;
    }
    let pageFront = pageNum - gap;
    if (pageFront < 1) {
        pageFront = 1;
    }
    for (let i = pageFront; i < pageNum; i++) {
        params.set("pageNum", i);
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
        params.set("pageNum", Number(pageNum) + 1);
        pagingContent += `<li class="page-item">
      <a class="page-link" href="${url}">Next</a>
    </li>`;
    }

    $("#pagination").html(pagingContent);
}