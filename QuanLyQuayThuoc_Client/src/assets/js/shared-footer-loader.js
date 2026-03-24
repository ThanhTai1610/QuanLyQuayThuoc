(function () {
  const mountNode = document.getElementById("homeFooterMount");
  if (!mountNode) return;

  const footerFallbackHtml = `
    <footer class="site-footer bg-light">
      <div class="container">
        <div class="row">
          <div class="col-md-6 col-lg-4 mb-4 mb-lg-0">
            <div class="block-7">
              <h3 class="footer-heading mb-4">Về <strong class="text-primary">Pharmative</strong></h3>
              <p>Pharmative là hệ thống nhà thuốc trực tuyến mang đến cho bạn giải pháp chăm sóc sức khỏe an toàn, tiện lợi và đáng tin cậy.</p>
            </div>
          </div>
          <div class="col-lg-3 mx-auto mb-5 mb-lg-0">
            <h3 class="footer-heading mb-4">Điều hướng</h3>
            <ul class="list-unstyled">
              <li><a href="#">Thực phẩm bổ sung</a></li>
              <li><a href="#">Vitamin</a></li>
              <li><a href="#">Ăn kiêng &amp; Dinh dưỡng</a></li>
              <li><a href="#">Trà &amp; Cà phê</a></li>
            </ul>
          </div>
          <div class="col-md-6 col-lg-4">
            <div class="block-5 mb-5">
              <h3 class="footer-heading mb-4">Thông tin liên hệ</h3>
              <ul class="list-unstyled">
                <li class="address">203 Fake St. Mountain View, San Francisco, California, USA</li>
                <li class="phone"><a href="tel://23923929210">+2 392 3929 210</a></li>
                <li class="email">emailaddress@domain.com</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </footer>
  `;

  fetch("footer.html")
    .then(function (response) { return response.text(); })
    .then(function (html) { mountNode.innerHTML = html; })
    .catch(function () { mountNode.innerHTML = footerFallbackHtml; });
})();
