var base = {
    configs: {
        pageSize: 10,
        pageIndex: 1
    },
    notify: function (message, type) {
        $.notify(message, {
            // whether to hide the notification on click
            clickToHide: true,
            // whether to auto-hide the notification
            autoHide: true,
            // if autoHide, hide after milliseconds
            autoHideDelay: 5000,
            // show the arrow pointing at the element
            arrowShow: true,
            // arrow size in pixels
            arrowSize: 5,
            // position defines the notification position though uses the defaults below
            position: '...',
            // default positions
            elementPosition: 'top right',
            globalPosition: 'top right',
            // default style
            style: 'bootstrap',
            // default class (string or [string])
            className: type,
            // show animation
            showAnimation: 'slideDown',
            // show animation duration
            showDuration: 400,
            // hide animation
            hideAnimation: 'slideUp',
            // hide animation duration
            hideDuration: 200,
            // padding between element and notification
            gap: 2,
            z_index: 3000
        });
    },
    confirm: function (message, okCallback) {
        bootbox.confirm({
            message: message,
            buttons: {
                confirm: {
                    label: translations.Yes,
                    className: 'btn-success'
                },
                cancel: {
                    label: translations.No,
                    className: 'btn-danger'
                }
            },
            className: 'modal-confirm',
            callback: function (result) {
                if (result === true) {
                    okCallback();
                }
            }
        });
    },
    dateFormatJson: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(datetime.substr(0, 10));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        return day + "/" + month + "/" + year;
    },

    formatDateJson: function (datetime) {
        if (!datetime) return '';

        var newdate = new Date(datetime.substr(0, 10));
        var year = newdate.getFullYear();
        var month = (newdate.getMonth() + 1).toString().padStart(2, '0');
        var day = newdate.getDate().toString().padStart(2, '0');

        return `${year}-${month}-${day}`; // Trả về định dạng YYYY-MM-DD
    },

    dateTimeFormatJson: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(datetime.substr(0, 19));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        var ss = newdate.getSeconds();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        if (ss < 10)
            ss = "0" + ss;
        return day + "/" + month + "/" + year + " " + hh + ":" + mm + ":" + ss;
    },
    startLoading: function () {
        if ($('.dv-loading').length > 0)
            $('.dv-loading').removeClass('hide');
    },
    stopLoading: function () {
        if ($('.dv-loading').length > 0)
            $('.dv-loading')
                .addClass('hide');
    },
    getStatus: function (status) {
        if (status == 1)
            return '<span class="badge bg-green">Active</span>';
        else
            return '<span class="badge bg-red">Block</span>';
    },
    formatNumber: function (number, precision) {
        if (!isFinite(number)) {
            return number.toString();
        }

        var a = number.toFixed(precision).split('.');
        a[0] = a[0].replace(/\d(?=(\d{3})+$)/g, '$&,');
        return a.join('.');
    },
    unflattern: function (arr) {
        var map = {};
        var roots = [];
        for (var i = 0; i < arr.length; i += 1) {
            var node = arr[i];
            node.children = [];
            map[node.id] = i; // use map to look-up the parents
            if (node.parentId !== null) {
                arr[map[node.parentId]].children.push(node);
            } else {
                roots.push(node);
            }
        }
        return roots;
    },
    convertToSlug: function (Text) {
        return Text
            .toLowerCase()
            .replace(/ /g, '-')
            .replace(/[^\w-]+/g, '');
    },
    wrapPaging: function (recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / base.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: paginationMultilingual.First,
            prev: paginationMultilingual.Prev,
            next: paginationMultilingual.Next,
            last: paginationMultilingual.Last,
            onPageClick: function (event, p) {
                if (base.configs.pageIndex !== p) {
                    base.configs.pageIndex = p;
                    setTimeout(callBack(), 200);
                }
            }
        })
    },
    setTitleModal: function (type) {
        if (type === 'add') {
            $('#modal-add-edit .modal-title').text(titleModalAddEditDetail.AddNew);
        } else if (type === 'edit') {
            $('#modal-add-edit .modal-title').text(titleModalAddEditDetail.Edit);
        } else if (type === 'detail') {
            $('#modal-add-edit .modal-title').text(titleModalAddEditDetail.Detail);
        }
    },
    sort: function (thElement, loadData) {
        let currentOrder = $(thElement).data("order") || "none";
        let newOrder;
        if (currentOrder === "none" || currentOrder === "desc") {
            newOrder = "asc";
        } else {
            newOrder = "desc";
        }
        $(thElement).data("order", newOrder);
        $("th i").removeClass("active").css("color", "black");
        if (newOrder === "desc") {
            $(thElement).find("i.mdi-arrow-up").css("color", "red");
        } else if (newOrder === "asc") {
            $(thElement).find("i.mdi-arrow-down").css("color", "red");
        }
        loadData();
    },
    getOrigin: function () {
        var origin = window.location.origin;
        return origin;
    },
    convertNumbers: function (input) {
        var stringNumber = input.toString();
        if (stringNumber.indexOf('.') !== -1) {
            var roundedNumber = parseFloat(input).toFixed(2);
            var replacedNumber = roundedNumber.replace('.', ',');
            return replacedNumber;
        } else {
            return input;
        }
    },
    onlyNumberInput: function (selector) {
        $(selector)
            .attr("min", "0")
            .on("keydown", function (e) {
                const allowedKeys = [
                    "Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete", "Home", "End"
                ];
                if (allowedKeys.includes(e.key)) { return; }

                if (["e", "E", "-"].includes(e.key)) {
                    e.preventDefault();
                    return;
                }

                if (e.key === ".") {
                    if ($(this).val().includes(".")) {
                        e.preventDefault();
                    }
                    return;
                }
                if (!/[0-9]/.test(e.key)) {
                    e.preventDefault();
                }
            });
    },

    flatPiker: function (id, minDate = null) {
        let input = $(id)[0];
        if (input._flatpickr) {
            input._flatpickr.destroy();
        }
        flatpickr(input, {
            enableTime: true,
            dateFormat: "Y-m-d H:i",
            time_24hr: true,
            locale: $('#culture').val() || "en",
            altInput: true,
            altFormat: "d F, Y H:i",
            minDate: minDate
        });
    },

    flatDatePiker: function (id) {
        let input = $(id)[0];
        if (input._flatpickr) {
            input._flatpickr.destroy();
        }
        flatpickr(input, {
            enableTime: false,
            dateFormat: "Y-m-d",
            locale: $('#culture').val() || "en",
            altInput: true,
            altFormat: "d F, Y",
            maxDate: "today",
        });
    },

    flatDateMinTodayPiker: function (id) {
        let input = $(id)[0];
        if (input._flatpickr) {
            input._flatpickr.destroy();
        }
        flatpickr(input, {
            enableTime: false,
            dateFormat: "Y-m-d",
            locale: $('#culture').val() || "en",
            altInput: true,
            altFormat: "d F, Y",
            minDate: "today",
        });
    },

    updateMinDate: function (startInput, endInput) {
        let startDate = $(startInput).val();
        if (startDate) {
            let endPicker = $(endInput)[0]._flatpickr;
            if (endPicker) {
                endPicker.set("minDate", startDate);
            }
        }
    },

    formatDateToSave: function (dateStr) {
        var lang = $('#culture').val() || 'en';
        let months = {
            "vn": ["Tháng một", "Tháng hai", "Tháng ba", "Tháng tư", "Tháng năm", "Tháng sáu", "Tháng bảy", "Tháng tám", "Tháng chín", "Tháng mười", "Tháng mười một", "Tháng mười hai"],
            "ja": ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
            "en": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
        };

        let regex = /([0-9]+)[^0-9]+([\w\s一-龥]+)/;
        let match = dateStr.match(regex);
        if (!match) return null;

        let day = match[1];
        let monthName = match[2].trim();
        let monthIndex = months[lang].indexOf(monthName);
        if (monthIndex === -1) return null;

        let year = dateStr.match(/\d{4}/)[0];
        let time = dateStr.match(/\d{2}:\d{2}/)[0];

        return `${year}-${(monthIndex + 1).toString().padStart(2, '0')}-${day.padStart(2, '0')}T${time}:00`;
    },

    formatDateToDisplay: function (isoStr) {
        var lang = $('#culture').val() || 'en';
        let months = {
            "vn": ["Tháng một", "Tháng hai", "Tháng ba", "Tháng tư", "Tháng năm", "Tháng sáu", "Tháng bảy", "Tháng tám", "Tháng chín", "Tháng mười", "Tháng mười một", "Tháng mười hai"],
            "ja": ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
            "en": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
        };

        let date = new Date(isoStr);
        let day = date.getDate();
        let month = months[lang][date.getMonth()];
        let year = date.getFullYear();
        let time = date.toTimeString().slice(0, 5);

        return `${day} ${month}, ${year} ${time}`;
    },
    formatNumberWhenInput: function (value) {
        value = value.replace(/\D/g, '');
        value = base.formatNumberForDisplay(value);
        return value;
    },
    formatNumberForDisplay: function (input) {
        let stringValue = `${base.convertNumberForDatabase(input || 0)}`;
        let isNegative = stringValue.startsWith('-');
        if (isNegative) {
            stringValue = stringValue.substring(1);
        }
        let parts = [];
        for (let i = stringValue.length; i > 0; i -= 3) {
            parts.unshift(stringValue.substring(Math.max(0, i - 3), i));
        }
        let formattedValue = parts.join(',');
        return isNegative ? `-${formattedValue}` : formattedValue;
    },
    convertNumberForDatabase: function (input) {
        let stringWithoutComma = input ? `${input}`.replace(/\,/g, '') : '0';
        let parsedFloat = parseFloat(stringWithoutComma);
        return parsedFloat;
    },
}

$(document).ajaxSend(function (e, xhr, options) {
    if (options.type.toUpperCase() == "POST" || options.type.toUpperCase() == "PUT") {
        var token = $('form').find("input[name='__RequestVerificationToken']").val();
        xhr.setRequestHeader("RequestVerificationToken", token);
    }
});

$.validator.addMethod("customNumber", function (value, element) {
    // Check if the field is empty then return true
    if (value.trim() === "") {
        return false;
    }
    // Checks if it is a number and is greater than or equal to 0
    return !isNaN(parseFloat(value)) && parseFloat(value) >= 0;
}, "Requires entering a number greater than or equal to 0");

$('.numeric-comma-input').on('input', function () {
    $(this).val($(this).val().replace(/[^0-9,]/g, ''));
});

$('body').on('click', '.dropbtn', function (e) {
    e.stopPropagation(); // Prevents the event from bubbling up to parent elements

    var $button = $(this);
    var $dropdown = $button.next('.dropdown-content');

    // If the dropdown has already been appended to the body, find it again
    if ($button.data('dropdownAppended')) {
        $dropdown = $('body').find('.dropdown-content[data-button-id="' + $button.attr('id') + '"]');
    } else {
        // If not appended yet, assign an ID and append it to the body
        var buttonId = 'dropbtn-' + Math.random().toString(36).substr(2, 9);
        $button.attr('id', buttonId);
        $dropdown.attr('data-button-id', buttonId);
        $('body').append($dropdown);
        $button.data('dropdownAppended', true);
    }

    // Check dropdown visibility
    if ($dropdown.is(':visible')) {
        $dropdown.hide(); // If already open, hide it
        return;
    }

    // Hide all other dropdowns before opening the current one
    $('.dropdown-content').not($dropdown).hide();

    // Get the exact position of the button
    var buttonOffset = $button.offset();
    var buttonHeight = $button.outerHeight();
    var dropdownHeight = $dropdown.outerHeight();

    // Check if there is a vertical scrollbar
    var hasVerticalScrollbar = document.documentElement.scrollHeight > document.documentElement.clientHeight;

    // Get the actual screen height
    var screenHeight = hasVerticalScrollbar ? $(window).height() : document.documentElement.clientHeight;

    // Check available space below and above the button
    var spaceBelow = screenHeight - buttonOffset.top - buttonHeight;
    var spaceAbove = buttonOffset.top;

    // If there is not enough space below, show dropdown above
    var topPosition;
    if (spaceBelow < dropdownHeight && spaceAbove > dropdownHeight) {
        topPosition = buttonOffset.top - dropdownHeight + 15; // Display above, 5px spacing
    } else {
        topPosition = buttonOffset.top + buttonHeight; // Display below, 5px spacing
    }

    // Set the dropdown position
    $dropdown.css({
        position: 'absolute',
        top: topPosition + 'px',
        left: buttonOffset.left - 10 + 'px',
        zIndex: 9999,
        display: 'block'
    });
});

// Hide dropdown when clicking on a dropdown menu item
$('body').on('click', '.dropdown-content a', function (e) {
    e.stopPropagation();
    $(this).closest('.dropdown-content').hide();
});

// Close dropdown when clicking outside of it
$('body').on('click', function (e) {
    if (!$(e.target).closest('.dropbtn').length && !$(e.target).closest('.dropdown-content').length) {
        $(".dropdown-content").hide();
    }
});

$(document).on('keypress', '.money-vn', function (event) {
    // Lấy mã phím được nhấn
    var keyCode = event.which || event.keyCode;

    // Cho phép các phím liên quan đến số hoặc các phím điều hướng (như mũi tên)
    if (!(keyCode >= 48 && keyCode <= 57) && keyCode != 46 && keyCode != 8 && keyCode != 9 && keyCode != 37 && keyCode != 39) {
        // Nếu không phải là số hoặc các phím điều hướng, ngăn không cho phép nhập
        event.preventDefault();
    }
});
$(document).on('input', '.money-vn', function () {
    let value = base.formatNumberWhenInput($(this).val());
    $(this).val(value);
});