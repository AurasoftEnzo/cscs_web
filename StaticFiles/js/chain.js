function chain(script) {
    fetch('/chain?program=' + script)
        .then(response => {
            if (response.status === 200) {
                console.log(window.location);
                
                window.location.replace('/' + script);
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

function chainBack(script) {
    fetch('/chain?program=' + script)
        .then(response => {
            if (response.status === 200) {
                window.location.replace('/' + script);
                return;
            } else {
                response.text().then(text => {
                    // console.log(text);

                    // document.body.appendChild(document.createElement('script')).text = text;
                    // // Alternatively, you can use:

                    document.open();
                    document.write(text);
                    document.close();
                });

                // return Promise.reject('Failed to load script');
            }
        });
    // .then(() => {
    //     // window.location.replace("https://aaaa.com");

    // });
}

