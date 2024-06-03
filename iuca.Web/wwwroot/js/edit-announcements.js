
// Other

const regex = new RegExp("\\[(\\d*)\\]", "g");

function setSelectpicker() {
    $('.selectpicker').selectpicker();
}

function setActiveButton(activeButton, nonActiveButtonsSelector) {
    // Найдем все неактивные кнопки по указанному селектору
    var nonActiveButtons = document.querySelectorAll(nonActiveButtonsSelector);

    // Уберем класс "active" у всех неактивных кнопок
    nonActiveButtons.forEach(function (button) {
        if (button.classList.contains('active')) {
            button.classList.remove('active');
        }
    });

    // Добавим класс "active" к выбранной активной кнопке
    if (!activeButton.classList.contains('active')) {
        activeButton.classList.add('active');
    }
}

// Sections modal window

function editSections(btn) {
    var announcementId = $(btn).val();

    setActiveButton(btn, 'button[name="editSectionsBtn"]');

    $.ajax({
        url: "/Announcements/GetSections",
        traditional: true,
        data: {
            "announcementId": announcementId
        },
        cache: false,
        success: function (html) {
            showSectionsModalWindow(html);
            setSelectpicker();
            updateSectionIndexes();
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
    return false;
}

function showSectionsModalWindow(html) {
    var modalContainer = $("#sectionsModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

// Edit sections

$(document).on('click', '#addAnnouncementSectionBtn', addSection);

function addSection() {
    var announcementId = $(this).val();

    $.ajax({
        url: '/Announcements/EditSection',
        type: 'get',
        data: {
            "announcementId": announcementId
        },
        cache: false,
        success: function (response) {
            var newResponse = addSectionNumber(response);
            $('#announcementSectionContainer').append(newResponse);
            setSelectpicker();
            updateSectionIndexes();
        }
    });
    return false;
}

function addSectionNumber(response) {
    var announcementSectionContainer = $('#announcementSectionContainer');
    var sections = announcementSectionContainer.find('input[name*="].Section"]');

    var maxValue = 0;
    sections.each(function () {
        var value = parseInt($(this).val(), 10);
        if (!isNaN(value) && value > maxValue) {
            maxValue = value;
        }
    });
    maxValue += 1;

    response = response.replace(/name="(.+?)\.Section"/g, 'name="$1.Section" value="' + maxValue + '"');
    return response;
}

$(document).on('click', '#removeAnnouncementSectionBtn', removeSection);

function removeSection() {
    $(this).parent().parent().parent().parent().parent().parent().remove();
    updateSectionIndexes();
}

function updateSectionIndexes() {

    var announcementSectionContainer = $('#announcementSectionContainer');
    var ids = announcementSectionContainer.find('input[name*="].Id"]');
    var announcementIds = announcementSectionContainer.find('input[name*="].AnnouncementId"]');
    var sections = announcementSectionContainer.find('input[name*="].Section"]');
    var credits = announcementSectionContainer.find('input[name*="].Credits"]');
    var places = announcementSectionContainer.find('input[name*="].Places"]');
    var shedules = announcementSectionContainer.find('input[name*="].Schedule"]');
    var instructorUserIds = announcementSectionContainer.find('select[name*="].InstructorUserId"]');
    var extraInstructorsJson = announcementSectionContainer.find('select[name*="].ExtraInstructorsJson"]');
    var groupsJson = announcementSectionContainer.find('select[name*="].GroupsJson"]');
    var length = ids.length;

    if (length > 0) {
        for (var i = 0; i < length; i++) {
            ids[i].name = ids[i].name.replace(regex, '[' + i + ']');
            announcementIds[i].name = announcementIds[i].name.replace(regex, '[' + i + ']');
            sections[i].name = sections[i].name.replace(regex, '[' + i + ']');
            credits[i].name = credits[i].name.replace(regex, '[' + i + ']');
            places[i].name = places[i].name.replace(regex, '[' + i + ']');
            shedules[i].name = shedules[i].name.replace(regex, '[' + i + ']');
            instructorUserIds[i].name = instructorUserIds[i].name.replace(regex, '[' + i + ']');
            extraInstructorsJson[i].name = extraInstructorsJson[i].name.replace(regex, '[' + i + ']');
            groupsJson[i].name = groupsJson[i].name.replace(regex, '[' + i + ']');
        }
    }
}

function setSectionCount() {
    var announcementSectionContainer = $('#announcementSectionContainer');
    var ids = announcementSectionContainer.find('input[name*="\\.Id"]');
    var count = ids.length;

    var activeButton = $('button[name="editSectionsBtn"].active');
    var elementForCount = activeButton.find('#sectionsCount');

    if (elementForCount.length > 0) {
        elementForCount.text(count);
    }
}

function scrollSectionsToTop() {
    var modal = document.getElementById("sectionsModalBody");
    modal.scrollTop = 0;
}

function saveSections() {
    setSelectpicker();
    updateSectionIndexes();
    //setSectionCount();
    scrollSectionsToTop();
}

// Instructors modal window

function editInstructors(btn) {
    var announcementId = $(btn).val();

    setActiveButton(btn, 'button[name="editInstructorsBtn"]');

    $.ajax({
        url: "/Announcements/GetInstructors",
        traditional: true,
        data: {
            "announcementId": announcementId
        },
        cache: false,
        success: function (html) {
            showInstructorsModalWindow(html);
            updateInstructorIndexes();
            setSelectpicker();
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
    return false;
}

function showInstructorsModalWindow(html) {
    var modalContainer = $("#instructorsModalContainer");
    modalContainer.html(html);
    modalContainer.find('.modal').modal('show');
}

// Edit instructors

$(document).on('click', '#addProposalInstructorBtn', addInstructor);

function addInstructor() {
    $.ajax({
        url: '/Announcements/EditInstructor',
        type: 'get',
        cache: false,
        success: function (response) {
            $('#proposalInstructorContainer').append(response);
            updateInstructorIndexes();
            setSelectpicker();
        }
    });
    return false;
}

$(document).on('click', '#removeProposalInstructorBtn', removeInstructor);

function removeInstructor() {
    $(this).parent().parent().parent().parent().remove();
    updateInstructorIndexes();
}

function updateInstructorIndexes() {

    var proposalInstructorContainer = $('#proposalInstructorContainer');
    var inputIds = proposalInstructorContainer.find('input[name*="].Id"]');
    var selectIds = proposalInstructorContainer.find('select[name*="].Id"]');
    var fullNames = proposalInstructorContainer.find('input[name*="].FullName"]');
    var firstLength = inputIds.length;
    var secondLength = selectIds.length;
    var totalLength = firstLength + secondLength;

    for (var i = 0; i < totalLength; i++) {
        if (i < firstLength) {
            inputIds[i].name = inputIds[i].name.replace(regex, '[' + i + ']');
            fullNames[i].name = fullNames[i].name.replace(regex, '[' + i + ']');
        } else {
            var index = i - firstLength;
            selectIds[index].name = selectIds[index].name.replace(regex, '[' + i + ']');
        }
    }
}

function setInstructorCount() {
    var proposalInstructorContainer = $('#proposalInstructorContainer');
    var inputIds = proposalInstructorContainer.find('input[name*="\\.Id"]');
    var selectIds = proposalInstructorContainer.find('select[name*="\\.Id"]');
    var count = inputIds.length + selectIds.length;

    var activeButton = $('button[name="editInstructorsBtn"].active');
    var elementForCount = activeButton.find('#instructorsCount');

    if (elementForCount.length > 0) {
        elementForCount.text(count);
    }
}

function saveInstructors() {
    setSelectpicker();
    updateInstructorIndexes();
    //setInstructorCount();
}

function replace(btn) {

    $(btn).prop("disabled", true);
    btn.innerHTML = '<i class="fas fa-spinner"></i>';

    if (btn.classList.contains("active")) {
        btn.classList.remove('active');
        removeReplaceForm(btn);
    } else {
        setActiveButton(btn, 'button[name="replaceProposalInstructorBtn"]');
        addReplaceForm(btn);
    } 
}

function addReplaceForm(btn) {
    $.ajax({
        url: '/Announcements/ReplaceInstructor',
        type: 'get',
        cache: false,
        success: function (response) {
            var container = btn.closest('.replace-container');
            $(container).append(response);
            setSelectpicker();
            $(btn).prop("disabled", false);
            btn.innerHTML = '<i class="fas fa-arrow-up"></i>';
        }
    });
    return false;
}

function removeReplaceForm(btn) {
    $(btn).parent().parent().next().remove();
    $(btn).prop("disabled", false);
    btn.innerHTML = '<i class="fas fa-arrow-down"></i>';
}

function replaceInstructors(btn) {
    var proposalInstructorContainer = $('#proposalInstructorContainer');
    var replaceContainer = btn.closest('.replace-container');

    var proposalCourseId = proposalInstructorContainer.find('input#proposalCourseId').val();
    var previousInstructorId = $(replaceContainer).find('input[name*=".Id"]').val();
    var futureInstructorId = $(replaceContainer).find('select#replaceProposalInstructorSelect').val();

    $.ajax({
        url: "/Announcements/ReplaceProposalInstructor",
        type: 'post',
        data: {
            "proposalCourseId": proposalCourseId,
            "previousInstructorId": previousInstructorId,
            "futureInstructorId": futureInstructorId
        },
        cache: false,
        success: function (data) {
            if (data.success) {

                var successMessage = "Successfully replaced!";
                var alert = document.getElementById("AnnouncementSuccessMessage");

                alert.classList.remove("d-none");
                alert.textContent = successMessage;

            } else {
                var alert = document.getElementById("AnnouncementErrorMessage");

                alert.classList.remove("d-none");
                alert.textContent = data.error;
            }
        },
        error: function (response) {
            console.log(response);
            showPopupModal(CONSTS.MODAL_FAIL, 'Error', 'An error occurred while replacing.');
        }
    });
    return false;
    
}

// Get controls

function getControls(btn) {
    var announcementId = $(btn).val();

    $.ajax({
        url: "/Announcements/GetControls",
        traditional: true,
        data: {
            "announcementId": announcementId
        },
        cache: false,
        success: function (html) {
            var container = $('#nav-controls-' + announcementId);
            container.html(html);
        },
        failure: function (response) {
            console.log(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
    return false;
}
