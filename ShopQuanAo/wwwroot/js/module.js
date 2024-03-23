//show product modal
export function showProductModal(productId) {
	// Lặp qua mỗi phần tử có class "itemData" là input chứa productId
	$('.itemData').each(function () {
		var dataId = this.value;
		// So sánh với itemId
		if (dataId === productId) {

			// Gửi AJAX request
			$.ajax({
				type: "POST",
				url: "/Products/ShowProductQuickViewModal",
				data: { productId: productId },
				success: function (response) {
					// Thêm nội dung modal vào DOM
					$('#js_productQuickViewModal').html(response);
					$(".js-select2").each(function () {
						$(this).select2({
							minimumResultsForSearch: 20,
							dropdownParent: $(this).next('.dropDownSelect2')
						});
					})
				},
				error: function (xhr, status, error) {
					// Xử lý lỗi nếu cần thiết
				}
			});
		}
	});

	// xử lý button cộng, trừ số lượng product trong quickview modal
	$(document).on('click', '.js_btnUpdateQuantity', function () {
		var parent = this.closest('.wrap-num-product');
		var quantity = parent.querySelector('.js_num_product');
		// Chuyển đổi giá trị từ chuỗi sang số
		var currentValue = parseInt(quantity.value);
		if (this.classList.contains('btn-minus')) {
			if (currentValue > 1)
				currentValue -= 1;
		}
		else {
			currentValue += 1;
		}
		// Cập nhật giá trị số lượng sản phẩm
		quantity.value = currentValue;
		quantity.dispatchEvent(new Event('input'));
	});
}
export function activeSlideImgProduct() {
	// product quick view modal. xu li chuyen doi hinh anh
	$(document).on('click', '.js_liimgProductItem', function () {
		var parent = this.closest('.wrap-slick3');
		var urlImageClick = this.querySelector('img').getAttribute('src');
		var imgMain = parent.querySelector('.js_imgProductMain');
		imgMain.setAttribute('src', urlImageClick);

	});
	// xu ly button chuyển đổi hình ảnh
	$(document).on('click', '.wrap-slick3-arrows button', function () {
		var parent = this.closest('.wrap-slick3');
		var imageGallery = parent.querySelectorAll('.js_imgProductItem');
		var imageMain = parent.querySelector('.js_imgProductMain');
		// Lặp qua từng phần tử trong danh sách
		var currentImage = 0, len = imageGallery.length - 1;
		for (var i = 0; i < imageGallery.length; i++) {
			// Kiểm tra xem phần tử hiện tại có phải là phần tử bạn đang tìm không
			if (imageGallery[i] === imageMain) {
				// Nếu là phần tử bạn đang tìm, in ra vị trí của nó và kết thúc vòng lặp
				currentImage = i;
				break;
			}
		}
		if (this.classList.contains('prev-slick3')) {
			if (currentImage > 0)
				currentImage--;
			imageMain.setAttribute('src', imageGallery[currentImage].getAttribute('src'))
		} else {
			if (currentImage < len)
				currentImage++;
			imageMain.setAttribute('src', imageGallery[currentImage].getAttribute('src'))
		}
	});
}

//------------------------------------------------------------------------------------------
// SEARCH
export function searchHome(element) {
	var parent = element.closest('.js-search-form');
	var keySearch = parent.querySelector('.js-key-search').value;

	$.ajax({
		type: "GET",
		url: "/Products/Fillter",
		dataType: "Json",
		data: { keySearch: keySearch },
		success: function (response) {
			window.location.href = response.redirectToUrl;
		},
		error: function (xhr, status, error) {
			// Xử lý lỗi nếu cần
		}
	});
}

// chức năng search trong page shop
export function searchShop(element) {
	var parent = element.closest('.js-search-form');
	var keySearch = element.value;
	if (keySearch.trim() != "") {
		$.ajax({
			type: "GET",
			url: "/Products/Fillter",
			data: { keySearch: keySearch },
			dataType: "Json",
			success: function (response) {
				window.location.href = response.redirectToUrl;
			},
			error: function (xhr, status, error) {
				// Xử lý lỗi nếu cần
			}
		});
	}
	if (keySearch == "") {
		window.location.href = "/products/shop";
	}
}


//---------------------------------------------------------------------
// REMOVE FROM CART
export function removeFromCart() {
	document.querySelectorAll(".js_header-cart-item-img").forEach(function (element) {
		element.addEventListener('click', function () {
			var cartId = this.getAttribute("data-cart-id");

			// Kiểm tra xem cartId có tồn tại hay không
			if (cartId) {
				// Gửi AJAX request
				$.ajax({
					type: "GET",
					url: "/Shopings/RemoveFromCart",
					data: { id: cartId },
					success: function (response) {
						location.reload();
					},
					error: function (xhr, status, error) {
						// Xử lý lỗi nếu cần thiết
					}
				});
			} else {
				console.error("Không có cartId được tìm thấy.");
			}
		});
	});
}


//--------------------------------------------------------------
// Update input quantity
export function updateInputQuantity() {
	// Lấy ra tất cả các nút "minus" và "plus"
	const btnMinusList = document.querySelectorAll('.btn-minus');
	const btnPlusList = document.querySelectorAll('.btn-plus');

	//Lặp qua tất cả các nút "minus" và thêm sự kiện click cho mỗi nút
	btnMinusList.forEach(btnMinus => {
		btnMinus.addEventListener('click', function () {
			// Lấy ra ô input tương ứng
			const input = btnMinus.closest('.wrap-num-product').querySelector('.num-product');
			console.log(input);
			let currentValue = parseInt(input.value);
			// Giảm giá trị đi 1, nhưng không cho phép giá trị nhỏ hơn 1
			if (currentValue > 1) {
				currentValue--;
			}
			// Cập nhật giá trị mới vào input
			input.value = currentValue;
			input.dispatchEvent(new Event('input'));

		});
	});

	// Lặp qua tất cả các nút "plus" và thêm sự kiện click cho mỗi nút
	btnPlusList.forEach(btnPlus => {
		btnPlus.addEventListener('click', function () {
			// Lấy ra ô input tương ứng
			const input = btnPlus.closest('.wrap-num-product').querySelector('.num-product');
			console.log(input);
			// Lấy giá trị hiện tại của input
			let currentValue = parseInt(input.value);
			// Tăng giá trị lên 1
			currentValue++;
			// Cập nhật giá trị mới vào input
			input.value = currentValue;
			input.dispatchEvent(new Event('input'));

		});
	});
}
//=============================================
//MENU MAIN
export function addClassActiveToMainMenu() {
	$(document).ready(function () {
		// Lấy chỉ số từ localStorage
		var activeMenuItemIndex = sessionStorage.getItem('menu_active_index');
		// Kiểm tra xem chỉ số có tồn tại không
		if (activeMenuItemIndex !== null) {
			// Tìm phần tử có chỉ số tương ứng và thêm class active
			$('#main-menu li').each(function () {
				if ($(this).hasClass('active-menu')) {
					$(this).removeClass('active-menu');
				}
			});
			$('#main-menu li').eq(activeMenuItemIndex).addClass('active-menu');
		}
	});
}

// FILTER
export function addClassActiveToFilterSelect() {
	$(document).ready(function () {
		// Lấy chỉ số từ localStorage
		var sortby = sessionStorage.getItem('sortBy');
		var priceZone = sessionStorage.getItem('priceZone');
		var color = sessionStorage.getItem('color');
		// Kiểm tra xem chỉ số có tồn tại không
		if (sortby !== null) {
			// Tìm phần tử có chỉ số tương ứng và thêm class active
			$('.filter-col1 a.filter-link-active').removeClass('filter-link-active'); // Xóa lớp active khỏi tất cả các liên kết

			// Tìm liên kết có chỉ số tương ứng và thêm lớp active
			$('.filter-col1 li').eq(parseInt(sortby)).find('a').addClass('filter-link-active');
		}
		if (priceZone !== null) {
			// Tìm phần tử có chỉ số tương ứng và thêm class active
			$('.filter-col2 a.filter-link-active').removeClass('filter-link-active'); // Xóa lớp active khỏi tất cả các liên kết

			// Tìm liên kết có chỉ số tương ứng và thêm lớp active
			$('.filter-col2 li').eq(parseInt(priceZone)).find('a').addClass('filter-link-active');
		}
		if (color !== null) {
			// Tìm phần tử có chỉ số tương ứng và thêm class active
			$('.filter-col3 a.filter-link-active').removeClass('filter-link-active'); // Xóa lớp active khỏi tất cả các liên kết

			// Tìm liên kết có chỉ số tương ứng và thêm lớp active
			$('.filter-col3 li').eq(parseInt(color)).find('a').addClass('filter-link-active');
		}
	});
}