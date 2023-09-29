function displayNames(input, target) {
    const output = document.getElementById(target);
    output.innerHTML = '';
    const files = input.files
    for (let i = 0; i < files.length; i++) {
        let file = files.item(i)
        let el = document.createElement('li');
        el.innerText = file.name;
        output.appendChild(el);
    }
}