const draggable_list_sideOne = document.getElementById('draggable-list-sideOne');
const draggable_list_sideTwo = document.getElementById('draggable-list-sideTwo');
const playAgainBtn = document.getElementById('play-button-opps');
const exitBtn = document.getElementById('exit-button-opps');
const popup = document.getElementById('popup-container-opps');
const finalMessage = document.getElementById('final-message-opps');

const check = document.getElementById('check');

const opponentsSideOne = document.getElementById("opponents-sideOne").value.split('_').filter(x=>x !== "");
const opponentsSideTwo = document.getElementById("opponents-sideTwo").value.split('_').filter(x=>x !== "");

const listItemsSideOne = [];
const listItemsSideTwo = [];

let dragStartIndexSideOne;
let dragStartIndexSideTwo;

createList(opponentsSideOne, listItemsSideOne, draggable_list_sideOne, "sideOne");
createList(opponentsSideTwo, listItemsSideTwo, draggable_list_sideTwo, "sideTwo");

function createList(opponentsSide, listItems, draggable_list, side) {
    [...opponentsSide]
        .map(a => ({ value: a, sort: Math.random() }))
        .sort((a, b) => a.sort - b.sort)
        .map(a => a.value)
        .forEach((person, index) => {
            const listItem = document.createElement('li');
            listItem.setAttribute('data-index', index);

            listItem.innerHTML = `
            <span class="number">${index + 1}</span>
            <div class="draggable ${side}" draggable="true">
                <p class="person-name">${person}</p>
            </div>
            `;

            listItems.push(listItem);
            draggable_list.appendChild(listItem);
        });

    addEventListeners();
}

function dragStartSideOne() {
    dragStartIndexSideOne = +this.closest('li').getAttribute('data-index');
}

function dragStartSideTwo() {
    dragStartIndexSideTwo = +this.closest('li').getAttribute('data-index');
}

function dragEnter() {
    this.classList.add('over');
}

function dragLeave() {
    this.classList.remove('over');
}

function dragOver(e) {
    e.preventDefault();
}

function dragDropSideOne() {
    const dragEndIndex = +this.getAttribute('data-index');
    swapItems(dragStartIndexSideOne, dragEndIndex, listItemsSideOne, ".draggable.sideOne");

    this.classList.remove('over');
}

function dragDropSideTwo() {
    const dragEndIndex = +this.getAttribute('data-index');
    swapItems(dragStartIndexSideTwo, dragEndIndex, listItemsSideTwo, ".draggable.sideTwo");

    this.classList.remove('over');
}

function swapItems(fromIndex, toIndex, listItems, className) {
    const itemOne = listItems[fromIndex].querySelector(className);
    const itemTwo = listItems[toIndex].querySelector(className);

    listItems[fromIndex].appendChild(itemTwo);
    listItems[toIndex].appendChild(itemOne);
}

function checkOrder() {

    var pairs = [];
    for (var i = 0; i < opponentsSideOne.length; i++) {

        var pair = opponentsSideOne[i] + "_" + opponentsSideTwo[i];
        pairs[i] = pair;
    }

    var score = 0;
    for (var i = 0; i < listItemsSideOne.length; i++) {

        var nameSideOne = listItemsSideOne[i].querySelector('.draggable.sideOne').innerText.trim();
        var nameSideTwo = listItemsSideTwo[i].querySelector('.draggable.sideTwo').innerText.trim();

        var pair = nameSideOne + "_" + nameSideTwo;
        if (!pairs.includes(pair)) {
            listItemsSideOne[i].classList.add('wrong');
            listItemsSideTwo[i].classList.add('wrong');
        }
        else {
            listItemsSideOne[i].classList.remove('wrong');
            listItemsSideTwo[i].classList.remove('wrong');

            listItemsSideOne[i].classList.add('right');
            listItemsSideTwo[i].classList.add('right');

            score++;
        }
    }

    if (score == listItemsSideOne.length) {
        finalMessage.innerText = 'Congratulations! You Won!';
        popup.style.display = 'flex';
    }
    else {
        finalMessage.innerText = 'Better luck next time!';
        popup.style.display = 'flex';
    }
}

function addEventListeners() {
    const draggablesSideOne = document.querySelectorAll('.draggable.sideOne');
    const dragListItemsSideOne = document.querySelectorAll('.draggable-list.sideOne li');

    draggablesSideOne.forEach(draggable => {
        draggable.addEventListener('dragstart', dragStartSideOne);
    });

    dragListItemsSideOne.forEach(item => {
        item.addEventListener('dragover', dragOver);
        item.addEventListener('drop', dragDropSideOne);
        item.addEventListener('dragenter', dragEnter);
        item.addEventListener('dragleave', dragLeave);
    });

    const draggablesSideTwo = document.querySelectorAll('.draggable.sideTwo');
    const dragListItemsSideTwo = document.querySelectorAll('.draggable-list.sideTwo li');

    draggablesSideTwo.forEach(draggable => {
        draggable.addEventListener('dragstart', dragStartSideTwo);
    });

    dragListItemsSideTwo.forEach(item => {
        item.addEventListener('dragover', dragOver);
        item.addEventListener('drop', dragDropSideTwo);
        item.addEventListener('dragenter', dragEnter);
        item.addEventListener('dragleave', dragLeave);
    });
}

check.addEventListener('click', checkOrder);

playAgainBtn.addEventListener('click', () => {
    popup.style.display = 'none';
    window.location.reload();
});

exitBtn.addEventListener('click', () => {
    popup.style.display = 'none';
    window.location.href = '/Games/Index';
});