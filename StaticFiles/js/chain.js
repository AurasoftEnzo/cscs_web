let chainCount = 1;

function chain(script) {
    // console.log(chainCount);

    if(dropdownMenu){
        dropdownMenu.style.display = 'none';
    }

    document.getElementById("page-overlay-" + chainCount).style.display = "flex";
    fetch('/chain?program=' + script)
        .then(response => {
            if (response.status === 200) {
                // console.log(window.location);

                // window.location.replace('/' + script);
                // NEGO
                htmx.ajax('GET', '/' + script, { target: '#page-overlay-' + chainCount, swap: 'innerHTML' }).then(() => {
                    chainCount++;
                    document.getElementById("page-overlay-").id = "page-overlay-" + chainCount;
                    document.getElementById("page-overlay-" + chainCount).style.zIndex = chainCount;
                });

                return;
            } else {
                response.text().then(html => {
                    // console.log(text);

                    // document.body.appendChild(document.createElement('script')).text = text;
                    // // Alternatively, you can use:

                    document.open();
                    document.write(html);
                    document.close();
                });

                // return Promise.reject('Failed to load script');
            }
        });
    // .then(() => {
    //     // window.location.replace("https://aaaa.com");

    // });
}

function chainBack() {
    // document.getElementById("page-overlay-" + chainCount).remove();

    document.getElementById("page-overlay-" + (chainCount - 1)).style.display = "none";
    document.getElementById("page-overlay-" + (chainCount - 1)).innerHTML = "";
    chainCount--;

    // let chainId = chainCount - 1;
    // document.getElementById("page-overlay-" + chainId).style.display = "flex";
    // document.getElementById("page-overlay-" + (chainId + 1)).remove();
    // let script = document.getElementById("page-overlay-" + (chainId + 1)).getAttribute("data-program");
    // console.log(script);
    // console.log(chainId);

    // fetch('/chainBack?chainId=' + chainId)
    //     .then(response => {
    //         if (response.status === 200) {
    //             window.location.replace('/' + script);
    //             return;
    //         } else {
    //             response.text().then(text => {
    //                 // console.log(text);

    //                 // document.body.appendChild(document.createElement('script')).text = text;
    //                 // // Alternatively, you can use:

    //                 document.open();
    //                 document.write(text);
    //                 document.close();
    //             });

    //             // return Promise.reject('Failed to load script');
    //         }
    //     });
    // .then(() => {
    //     // window.location.replace("https://aaaa.com");

    // });
}

